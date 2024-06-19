using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace First2DGame;

public class UserInfoManager
{
    private static UserInfoManager _instsance;

    public static UserInfoManager Instance => _instsance ??= new UserInfoManager();

    public string GetUserUniqueName()
    {
        var deviceMacAddress = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where( nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback )
            .Select( nic => nic.GetPhysicalAddress().ToString() )
            .FirstOrDefault();
        var username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        var userUniqueName = deviceMacAddress + username;
        
        var hashAlgorithm = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(userUniqueName);
        byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
    
    private UserInfoManager(){}
}
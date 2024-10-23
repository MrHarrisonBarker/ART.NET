using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ART.NET;

public record class NetworkInterface
{
    public string Name { get; }
    public IPAddress Address { get; }
    public IPAddress Subnet { get; }

    public NetworkInterface( string name, IPAddress address, IPAddress subnet )
    {
        Name = name;
        Address = address;
        Subnet = subnet;

        var addressBytes = address.GetAddressBytes();
        var subnetBytes = subnet.GetAddressBytes();

        for ( var i = 0; i < 4; i++ )
        {
            addressBytes[ i ] = ( byte )( ( uint )addressBytes[ i ] | ( ( uint )subnetBytes[ i ] ^ ( uint )255 ) );
        }

        BroadcastAddress = new IPAddress( addressBytes );
    }

    public IPAddress BroadcastAddress { get; }
    
    public static IEnumerable<NetworkInterface> AvailableInterfaces
    {
        get
        {
            var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            var supportedInterfaces = interfaces
                .Where( networkInterface => networkInterface is
                    { SupportsMulticast: true, OperationalStatus: OperationalStatus.Up } );

            foreach ( var supportedInterface in supportedInterfaces )
            {
                foreach ( var uniCastAddress in supportedInterface.GetIPProperties()
                             .UnicastAddresses )
                {
                    if ( uniCastAddress.Address.AddressFamily == AddressFamily.InterNetwork )
                        yield return new ART.NET.NetworkInterface(
                            supportedInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback
                                ? "Local host"
                                : supportedInterface.Description, uniCastAddress.Address, uniCastAddress.IPv4Mask );
                }
            }
        }
    }
};
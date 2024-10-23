using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ART.NET;

public class ArtNetSocket : Socket
{
    private const int Port = 6454;

    public readonly NetworkInterface NetworkInterface;
    private readonly EndPoint BroadcastEndpoint;

    private ArtNetOpCodes[] ListeningFor = [ ];

    private readonly byte[] RxBuffer = new byte[ 1024 ];

    public readonly BlockingCollection<ArtNetPacketBuffer> RxQueue = new ();

    public ArtNetSocket( NetworkInterface networkInterface ) : base( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp )
    {
        NetworkInterface = networkInterface;
        BroadcastEndpoint = new IPEndPoint( networkInterface.BroadcastAddress, Port );
        EnableBroadcast = true;
        MulticastLoopback = false;
        SendTimeout = 0;

        SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true );
        SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1 );
    }

    public void Send( IArtNetPacketBuffer buffer, IPAddress? destination = null )
    {
        try
        {
            // Console.WriteLine( $"Sending to {destination}" );
            SendTo( buffer.Buffer, SocketFlags.None, destination is not null ? new IPEndPoint( destination, Port ) : BroadcastEndpoint );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }
    }

    public void ListenFor( params ArtNetOpCodes[] opCodes )
    {
        Bind( new IPEndPoint( IPAddress.Any, Port ) );

        ListeningFor = opCodes;

        StartListening();
    }

    private void StartListening()
    {
        try
        {
            // Console.WriteLine( "Starting to listen" );
            EndPoint localPort = new IPEndPoint( IPAddress.Any, Port );
            BeginReceiveMessageFrom( RxBuffer, 0, RxBuffer.Length, SocketFlags.None, ref localPort, OnReceive, null );
        }
        catch ( Exception e )
        {
            Console.WriteLine( "Failed to start listening {0}", e );
        }
    }

    private void OnReceive( IAsyncResult ar )
    {
        try
        {
            if ( ListeningFor.Length != 0 )
            {
                EndPoint remote = new IPEndPoint( IPAddress.Any, 0 );
                SocketFlags flags = SocketFlags.None;

                var length = EndReceiveMessageFrom( ar, ref flags, ref remote, out _ );

                // Console.WriteLine($"Received {length} bytes from {remote}");
                
                if ( NetworkInterface.Address.Equals( ( ( IPEndPoint )remote ).Address ) ) return;
                if ( length <= 12 || !RxBuffer.IsArtNetPacket() ) return;

                var opCode = RxBuffer.OpCode();
                Console.WriteLine( opCode );

                if ( ListeningFor.Contains( opCode ) )
                {
                    Console.WriteLine( $"Got {length} bytes from socket" );

                    var packetBuffer = ArtNetPacketBuffer.Parse( RxBuffer, opCode );

                    if ( packetBuffer is not null )
                    {
                        Console.WriteLine( packetBuffer );
                        RxQueue.Add( packetBuffer );
                    }
                }
            }
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }
        finally
        {
            StartListening();
        }
    }
}

public static class BufferExtensions
{
    public static bool IsArtNetPacket( this byte[] buffer )
    {
        return
            buffer[ 0 ] == 65 &&
            buffer[ 1 ] == 114 &&
            buffer[ 2 ] == 116 &&
            buffer[ 3 ] == 45 &&
            buffer[ 4 ] == 78 &&
            buffer[ 5 ] == 101 &&
            buffer[ 6 ] == 116 &&
            buffer[ 7 ] == 0;
    }

    public static ArtNetOpCodes OpCode( this byte[] buffer )
    {
        return ( ArtNetOpCodes )( buffer[ 9 ] + ( buffer[ 8 ] << 8 ) );
    }
}
using System.Net;

namespace ART.NET;

public class ArtNetNodeManager
{
    private const int PollRate = 1000 * 30;
    private readonly ArtNetSocket Socket;

    private readonly Thread PollThread;
    private readonly Thread ListenThread;

    private readonly CancellationTokenSource CancellationTokenSource = new ();

    public ArtNetNodeManager( ArtNetSocket socket, string shortName, string longName )
    {
        Socket = socket;
        PollReplyBuffer = new ArtNetPollReplyBuffer( socket.NetworkInterface.Address, shortName, longName );
        Nodes = new Dictionary<IPAddress, ArtNetNode>();

        PollThread = new Thread( Poll )
        {
            Name = "PollThread",
            Priority = ThreadPriority.BelowNormal
        };

        ListenThread = new Thread( Listen )
        {
            Name = "ListenThread",
            Priority = ThreadPriority.BelowNormal
        };

        PollThread.Start();
        ListenThread.Start();
    }

    private readonly ArtNetPollBuffer PollBuffer = new ();
    private readonly ArtNetPollReplyBuffer PollReplyBuffer;

    public Dictionary<IPAddress, ArtNetNode> Nodes { get; }

    private void Poll()
    {
        while ( !CancellationTokenSource.IsCancellationRequested )
        {
            Console.WriteLine( "Sending ArtNetPoll" );
            Socket.Send( PollBuffer );
            Socket.Send( PollReplyBuffer );

            Thread.Sleep( PollRate );
        }
    }

    private void Listen()
    {
        while ( !CancellationTokenSource.IsCancellationRequested )
        {
            Console.WriteLine( "Listening for polls" );
            var nextBuffer = Socket.RxQueue.Take();

            if ( nextBuffer is ArtNetPollBuffer )
            {
                Console.WriteLine( "Received poll, replying" );
                Socket.Send( PollReplyBuffer );
            }
            else if ( nextBuffer is ArtNetPollReplyBuffer reply )
            {
                var ip = new IPAddress( reply.IpAddress );
                if ( Nodes.ContainsKey( ip ) )
                {
                    Nodes[ ip ] = new ArtNetNode( reply.ShortName, reply.LongName, ip );
                }
                else
                {
                    Nodes.Add( ip, new ArtNetNode( reply.ShortName, reply.LongName, ip ) );
                }
                
                Console.WriteLine( $"Nodes -> {string.Join( ",", Nodes )}" );
            }
        }
    }
}

public record ArtNetNode( string ShortName, string LongName, IPAddress Address )
{
    public override string ToString()
    {
        return $"{Address}:{LongName}";
    }
}
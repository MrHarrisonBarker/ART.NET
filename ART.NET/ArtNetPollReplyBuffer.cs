using System.Net;
using System.Text;

namespace ART.NET;

public sealed class ArtNetPollReplyBuffer : ArtNetPacketBuffer
{
    public override byte[] Buffer { get; } = new byte[ 256 ];


    public ArtNetPollReplyBuffer( IPAddress destination, string shortName, string longName ) : base( ArtNetOpCodes.PollReply )
    {
        var destinationBytes = destination.GetAddressBytes();
        var shortNameBytes = Encoding.Default.GetBytes( shortName );
        var longNameBytes = Encoding.Default.GetBytes( longName );

        System.Buffer.BlockCopy( destinationBytes, 0, Buffer, 10, 4 );
        System.Buffer.BlockCopy( shortNameBytes, 0, Buffer, 26, shortNameBytes.Length );
        System.Buffer.BlockCopy( longNameBytes, 0, Buffer, 44, longNameBytes.Length );

        Buffer[ 20 ] = 255;
        Buffer[ 21 ] = 255;
    }

    public ArtNetPollReplyBuffer( byte[] buffer )
    {
        System.Buffer.BlockCopy( buffer, 0, Buffer, 0, Buffer.Length );
    }

    public byte[] IpAddress => Buffer[ 10..14 ];
    public string ShortName => Encoding.Default.GetString( Buffer[ 26..44 ] ).TrimEnd( '\0' );
    public string LongName => Encoding.Default.GetString( Buffer[ 44..108 ] ).TrimEnd( '\0' );
    public byte[] Report => Buffer[ 108..172 ];

    public override string ToString()
    {
        return $"[ARTNET-POLL-REPLY] {string.Join( ",", IpAddress )} {ShortName}\n{LongName}";
    }
}
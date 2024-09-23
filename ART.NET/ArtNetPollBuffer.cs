namespace ART.NET;

public sealed class ArtNetPollBuffer : ArtNetPacketBuffer
{
    public override byte[] Buffer { get; } = new byte[ 14 ];


    public ArtNetPollBuffer() : base( ArtNetOpCodes.Poll )
    {
        Buffer[ 12 ] = 0b00000000;
        Buffer[ 13 ] = 0x80;
    }

    public ArtNetPollBuffer( byte[] buffer )
    {
        System.Buffer.BlockCopy( buffer, 0, Buffer, 0, Buffer.Length );
    }
}
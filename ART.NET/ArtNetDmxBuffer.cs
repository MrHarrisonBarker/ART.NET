namespace ART.NET;

public sealed class ArtNetDmxBuffer : ArtNetPacketBuffer
{
    // ArtDmx Header : 18 bytes
    // ArtDmx Data : 512 bytes
    // ArtDmx Payload : 530 bytes

    public override byte[] Buffer { get; } = new byte[ 530 ];

    public ArtNetDmxBuffer(): base( ArtNetOpCodes.Dmx )
    {
        SetSequence( 0 );

        Buffer[ 13 ] = 0; // Physical

        SetUniverse( 0 );

        Buffer[ 16 ] = 2; // Length High
        Buffer[ 17 ] = 0; // Length Low
    }

    public void SetUniverse( short universe )
    {
        Buffer[ 14 ] = ( byte )( universe >> 0x00 & 0xFF ); // SubUni ( Universe Low )
        Buffer[ 15 ] = ( byte )( universe >> 0x08 & 0xFF ); // Net ( Universe High )
    }

    public void SetSequence( byte sequence )
    {
        Buffer[ 12 ] = sequence;
    }

    public void SetData( byte[] data )
    {
        System.Buffer.BlockCopy( data, 0, Buffer, 18, 512 );
    }
}
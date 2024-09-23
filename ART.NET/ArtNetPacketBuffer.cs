namespace ART.NET;

public abstract class ArtNetPacketBuffer : IArtNetPacketBuffer
{
    public abstract byte[] Buffer { get; }

    private void SetProtocol()
    {
        // Protocol
        Buffer[ 0 ] = 65;
        Buffer[ 1 ] = 114;
        Buffer[ 2 ] = 116;
        Buffer[ 3 ] = 45;
        Buffer[ 4 ] = 78;
        Buffer[ 5 ] = 101;
        Buffer[ 6 ] = 116;
        Buffer[ 7 ] = 0;

        Buffer[ 10 ] = 0; // Protocol Version High
        Buffer[ 11 ] = 14; // Protocol Version Low
    }

    private void SetOp( ArtNetOpCodes opCode )
    {
        Buffer[ 8 ] = ( byte )( ( short )opCode >> 0x08 ); // OpCode Low
        Buffer[ 9 ] = ( byte )( ( short )opCode >> 0x00 ); // OpCode High
    }

    protected ArtNetPacketBuffer( ArtNetOpCodes opCode )
    {
        SetProtocol();
        SetOp( opCode );
    }

    protected ArtNetPacketBuffer()
    {
    }

    public static ArtNetPacketBuffer? Parse( byte[] buffer, ArtNetOpCodes opCode )
    {
        switch ( opCode )
        {
            case ArtNetOpCodes.Poll:
                return new ArtNetPollBuffer( buffer );
            case ArtNetOpCodes.PollReply:
                return new ArtNetPollReplyBuffer( buffer );
            case ArtNetOpCodes.None:
            case ArtNetOpCodes.Dmx:
            case ArtNetOpCodes.Sync:
            case ArtNetOpCodes.TodRequest:
            case ArtNetOpCodes.TodData:
            case ArtNetOpCodes.TodControl:
            case ArtNetOpCodes.Rdm:
            case ArtNetOpCodes.RdmSub:
            case ArtNetOpCodes.ArtTrigger:
            case ArtNetOpCodes.Address:
            case ArtNetOpCodes.IpProg:
            case ArtNetOpCodes.IpProgReply:
            case ArtNetOpCodes.Input:
            default:
                return null;
        }
    }
}
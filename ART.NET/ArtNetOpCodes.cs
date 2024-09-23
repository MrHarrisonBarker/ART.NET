namespace ART.NET;

public enum ArtNetOpCodes : short
{
    None = 0x00,
    Poll = 0x20,
    PollReply = 0x21,
    Dmx = 0x50,
    Sync = 0x52,
    TodRequest = 0x80,
    TodData = 0x81,
    TodControl = 0x82,
    Rdm = 0x83,
    RdmSub = 0x84,
    ArtTrigger = 0x99,

    Address = 0x60,
    IpProg = 0xF8,
    IpProgReply = 0xF9,

    Input = 0x70
}
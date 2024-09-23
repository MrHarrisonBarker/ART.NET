// See https://aka.ms/new-console-template for more information

using System.Net;
using ART.NET;

Console.WriteLine( "Hello, World!" );

var socket = new ArtNetSocket( new NetworkInterface( "TEST", IPAddress.Parse( "192.168.1.2" ), IPAddress.Parse( "255.255.255.0" ) ) );
socket.ListenFor( ArtNetOpCodes.Poll, ArtNetOpCodes.PollReply );
// var pollBuffer = new ArtNetPollBuffer();
// for ( int i = 0; i < 10; i++ )
// {
//     socket.Send( pollBuffer );
// }

// await Task.Run( () =>
// {
//
     while ( true )
     {

         var next = socket.RxQueue.Take();

         if ( next is ArtNetPollBuffer poll )
         {
            var pollReply = new ArtNetPollReplyBuffer( IPAddress.Parse( "192.168.1.2" ), "LGFX", "LGFX SMOKE CONTROLLER" );
            var pollReply2 = new ArtNetPollReplyBuffer( IPAddress.Parse( "192.168.1.2" ), "LGFX", "LGFX PRISMA" );

            socket.Send( pollReply );
            socket.Send( pollReply2 );
         }
     }
//
//     return;
// } );

// var dmxBuffer = new ArtNetDmxBuffer();
//
// var d = new byte[ 512 ];
// for ( int i = 0; i < 512; i++ )
// {
//     d[ i ] = 0;
// }
//
// dmxBuffer.SetData( d );
// dmxBuffer.SetUniverse( 1 );
//
// for ( byte i = 0; i < 255; i++ )
// {
//     dmxBuffer.SetSequence( i );
//     socket.Send( dmxBuffer );
//     await Task.Delay( 200 );
// }


Console.ReadLine();
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ART.NET.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        // var buffer = new ArtNetDmxBuffer();
        //
        // var d = new byte[ 512 ];
        // for ( int i = 0; i < 512; i++ )
        // {
        //     d[ i ] = 128;
        // }
        //
        // buffer.SetData( d );
        // buffer.SetUniverse( 1 );
        //
        var socket = new ArtNetSocket( new NetworkInterface( "TEST", IPAddress.Parse( "192.168.1.3" ), IPAddress.Parse( "255.255.255.0" ) ) );
        socket.ListenFor( ArtNetOpCodes.Poll, ArtNetOpCodes.PollReply );
        //
        // for ( byte i = 0; i < 255; i++ )
        // {
        //     buffer.SetSequence( i );
        //     socket.Send( buffer );
        // }

        var pollBuffer = new ArtNetPollBuffer();

        // while ( 1 > 0 )
        // {
        for ( int i = 0; i < 10; i++ )
        {
            socket.Send( pollBuffer );
        }

        // var buffer = new byte[ 512 ];

        // while ( 1 < 2 )
        // {
        //     var len = socket.Receive( buffer );
        //
        //     if ( len > 22 )
        //     {
        //         var ipAddress = new byte[ 4 ];
        //         var portName = new byte[ 18 ];
        //         var longName = new byte[ 64 ];
        //
        //         Buffer.BlockCopy( buffer, 10, ipAddress, 0, 4 );
        //         Buffer.BlockCopy( buffer, 26, portName, 0, 18 );
        //         Buffer.BlockCopy( buffer, 44, longName, 0, 64 );
        //
        //         Console.WriteLine( string.Join( ",", ipAddress ) );
        //         Console.WriteLine( Encoding.ASCII.GetString( portName ) );
        //         Console.WriteLine( Encoding.ASCII.GetString( longName ) );
        //     }
        //
        //     Console.WriteLine( Encoding.UTF8.GetString( buffer ) );
        // }

        // await Task.Delay( 2000 );
        // }

        // }

        // while ( true )
        // {
        // var newPacket = socket.RxQueue.Take();
        // Console.WriteLine( $"Got new packet!, {newPacket}" );
        // }


        short uni = 0x5000;

        Console.WriteLine( uni >> 0x00 & 0xFF );
        Console.WriteLine( uni >> 0x08 & 0xFF );


        Assert.Pass();
    }
}
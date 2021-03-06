﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Swaelo_Server
{
    internal static class NavMeshNodeIDGenerator
    {
        private static readonly string Encode = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        private static long PreviousNavMeshNodeID = DateTime.UtcNow.Ticks;
        public static string GetNextID() => GenerateNavMeshNodeID(Interlocked.Increment(ref PreviousNavMeshNodeID));
        private static unsafe string GenerateNavMeshNodeID(long ID)
        {
            char* CharBuffer = stackalloc char[13];
            CharBuffer[0] = Encode[(int)(ID >> 60) & 31];
            CharBuffer[1] = Encode[(int)(ID >> 55) & 31];
            CharBuffer[2] = Encode[(int)(ID >> 50) & 31];
            CharBuffer[3] = Encode[(int)(ID >> 45) & 31];
            CharBuffer[4] = Encode[(int)(ID >> 40) & 31];
            CharBuffer[5] = Encode[(int)(ID >> 35) & 31];
            CharBuffer[6] = Encode[(int)(ID >> 30) & 31];
            CharBuffer[7] = Encode[(int)(ID >> 25) & 31];
            CharBuffer[8] = Encode[(int)(ID >> 20) & 31];
            CharBuffer[9] = Encode[(int)(ID >> 15) & 31];
            CharBuffer[10] = Encode[(int)(ID >> 10) & 31];
            CharBuffer[11] = Encode[(int)(ID >> 5) & 31];
            CharBuffer[12] = Encode[(int)ID & 31];

            return new string(CharBuffer, 0, 13);
        }
    }
}


/*
 * using System;
using System.Threading;

namespace Swaelo_Server
{
    internal static class EntityIDGenerator
    {
        private static readonly string Encode = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        private static long PreviousEntityID = DateTime.UtcNow.Ticks;
        public static string GetNextID() => GenerateEntityID(Interlocked.Increment(ref PreviousEntityID));
        private static unsafe string GenerateEntityID(long ID)
        {
            char* CharBuffer = stackalloc char[13];
            CharBuffer[0] = Encode[(int)(ID >> 60) & 31];
            CharBuffer[1] = Encode[(int)(ID >> 55) & 31];
            CharBuffer[2] = Encode[(int)(ID >> 50) & 31];
            CharBuffer[3] = Encode[(int)(ID >> 45) & 31];
            CharBuffer[4] = Encode[(int)(ID >> 40) & 31];
            CharBuffer[5] = Encode[(int)(ID >> 35) & 31];
            CharBuffer[6] = Encode[(int)(ID >> 30) & 31];
            CharBuffer[7] = Encode[(int)(ID >> 25) & 31];
            CharBuffer[8] = Encode[(int)(ID >> 20) & 31];
            CharBuffer[9] = Encode[(int)(ID >> 15) & 31];
            CharBuffer[10] = Encode[(int)(ID >> 10) & 31];
            CharBuffer[11] = Encode[(int)(ID >> 5) & 31];
            CharBuffer[12] = Encode[(int)ID & 31];

            return new string(CharBuffer, 0, 13);
        }
    }
}

 * */

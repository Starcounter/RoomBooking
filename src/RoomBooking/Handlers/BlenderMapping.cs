using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomBooking
{
    static class BlenderMapping
    {
        public static void Register()
        {
            Blender.MapUri("/RoomBooking/menu", "menu");
        }
    }
}
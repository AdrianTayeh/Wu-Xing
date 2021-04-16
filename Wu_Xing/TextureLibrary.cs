﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wu_Xing
{
    static class TextureLibrary
    {
        //Folder Backgrounds
        public static Texture2D BackgroundGray { get; private set; }
        public static Texture2D BackgroundRed { get; private set; }
        public static Texture2D BackgroundSplit { get; private set; }

        //Folder Characters
        public static Texture2D Adam { get; private set; }

        //Folder Doors
        public static Dictionary<Room.Type, Texture2D> DoorBottoms { get; private set; }
        public static Dictionary<Room.Type, Texture2D> DoorTops { get; private set; }
        public static Dictionary<Room.Type, Texture2D> DoorFronts { get; private set; }

        //Folder Elements
        public static Texture2D EnergyCircle { get; private set; }
        public static Texture2D EnergyLine { get; private set; }
        public static Texture2D EnergyMask { get; private set; }
        public static Texture2D GemEarthHidden { get; private set; }
        public static Texture2D GemEarth { get; private set; }
        public static Texture2D GemFireHidden { get; private set; }
        public static Texture2D GemFire { get; private set; }
        public static Texture2D GemMetalHidden { get; private set; }
        public static Texture2D GemMetal { get; private set; }
        public static Texture2D GemWaterHidden { get; private set; }
        public static Texture2D GemWater { get; private set; }
        public static Texture2D GemWoodHidden { get; private set; }
        public static Texture2D GemWood { get; private set; }
        public static Texture2D SymbolEarth { get; private set; }
        public static Texture2D SymbolFire { get; private set; }
        public static Texture2D SymbolMetal { get; private set; }
        public static Texture2D SymbolWater { get; private set; }
        public static Texture2D SymbolWood { get; private set; }

        //Folder Icons
        public static Texture2D IconDelete { get; private set; }
        public static Texture2D IconPlus { get; private set; }

        //Folder Logos
        public static Texture2D WXLogoDarkDots { get; private set; }
        public static Texture2D WXLogoDark { get; private set; }
        public static Texture2D WXLogoIcon { get; private set; }
        public static Texture2D WXLogoLightDots { get; private set; }
        public static Texture2D WXLogoLight { get; private set; }

        //Folder Rooms
        public static Dictionary<string, Texture2D> Rooms { get; private set; }

        //Generated by code
        public static Texture2D WhitePixel { get; private set; }

        public static void Load(ContentManager Content, GraphicsDevice GraphicsDevice)
        {
            //Folder Backgrounds
            BackgroundGray = Content.Load<Texture2D>("Backgrounds\\Background Gray");
            BackgroundRed = Content.Load<Texture2D>("Backgrounds\\Background Red");
            BackgroundSplit = Content.Load<Texture2D>("Backgrounds\\Background Split");

            //Folder Characters
            Adam = Content.Load<Texture2D>("Characters\\Adam");

            //Folder Doors
            DoorBottoms = new Dictionary<Room.Type, Texture2D>();
            DoorBottoms.Add(Room.Type.Normal, Content.Load<Texture2D>("Doors\\Door Bottom Normal"));
            DoorBottoms.Add(Room.Type.Boss, Content.Load<Texture2D>("Doors\\Door Bottom Boss"));

            DoorTops = new Dictionary<Room.Type, Texture2D>();
            DoorTops.Add(Room.Type.Normal, Content.Load<Texture2D>("Doors\\Door Top Normal"));
            DoorTops.Add(Room.Type.Boss, Content.Load<Texture2D>("Doors\\Door Top Boss"));

            DoorFronts = new Dictionary<Room.Type, Texture2D>();
            DoorFronts.Add(Room.Type.Normal, Content.Load<Texture2D>("Doors\\Door Front Normal"));
            DoorFronts.Add(Room.Type.Boss, Content.Load<Texture2D>("Doors\\Door Front Normal"));

            //Folder Elements
            EnergyCircle = Content.Load<Texture2D>("Elements\\Energy Circle");
            EnergyLine = Content.Load<Texture2D>("Elements\\Energy Line");
            EnergyMask = Content.Load<Texture2D>("Elements\\Energy Mask");
            GemEarthHidden = Content.Load<Texture2D>("Elements\\Gem Earth Hidden");
            GemEarth = Content.Load<Texture2D>("Elements\\Gem Earth");
            GemFireHidden = Content.Load<Texture2D>("Elements\\Gem Fire Hidden");
            GemFire = Content.Load<Texture2D>("Elements\\Gem Fire");
            GemMetalHidden = Content.Load<Texture2D>("Elements\\Gem Metal Hidden");
            GemMetal = Content.Load<Texture2D>("Elements\\Gem Metal");
            GemWaterHidden = Content.Load<Texture2D>("Elements\\Gem Water Hidden");
            GemWater = Content.Load<Texture2D>("Elements\\Gem Water");
            GemWoodHidden = Content.Load<Texture2D>("Elements\\Gem Wood Hidden");
            GemWood = Content.Load<Texture2D>("Elements\\Gem Wood");
            SymbolEarth = Content.Load<Texture2D>("Elements\\Symbol Earth");
            SymbolFire = Content.Load<Texture2D>("Elements\\Symbol Fire");
            SymbolMetal = Content.Load<Texture2D>("Elements\\Symbol Metal");
            SymbolWater = Content.Load<Texture2D>("Elements\\Symbol Water");
            SymbolWood = Content.Load<Texture2D>("Elements\\Symbol Wood");

            //Folder Icons
            IconDelete = Content.Load<Texture2D>("Icons\\Icon Delete");
            IconPlus = Content.Load<Texture2D>("Icons\\Icon Plus");

            //Folder Logos
            WXLogoDarkDots = Content.Load<Texture2D>("Logos\\WX Logo Dark Dots");
            WXLogoDark = Content.Load<Texture2D>("Logos\\WX Logo Dark");
            WXLogoIcon = Content.Load<Texture2D>("Logos\\WX Logo Icon");
            WXLogoLightDots = Content.Load<Texture2D>("Logos\\WX Logo Light Dots");
            WXLogoLight = Content.Load<Texture2D>("Logos\\WX Logo Light");

            //Folder Rooms
            Rooms = new Dictionary<string, Texture2D>();
            Rooms.Add("1x1", Content.Load<Texture2D>("Rooms\\Room 1x1"));
            Rooms.Add("1x2", Content.Load<Texture2D>("Rooms\\Room 1x2"));
            Rooms.Add("1x3", Content.Load<Texture2D>("Rooms\\Room 1x3"));
            Rooms.Add("2x1", Content.Load<Texture2D>("Rooms\\Room 2x1"));
            Rooms.Add("3x1", Content.Load<Texture2D>("Rooms\\Room 3x1"));
            Rooms.Add("2x2", Content.Load<Texture2D>("Rooms\\Room 2x2"));

            //Generated by code
            WhitePixel = new Texture2D(GraphicsDevice, 2, 2);
            Color[] c = new Color[4];
            c[0] = c[1] = c[2] = c[3] = Color.White;
            WhitePixel.SetData(c);
        }
    }
}

using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Wu_Xing
{
    static class SoundLibrary
    {
        public static SoundEffect AdamDeath { get; private set; }
        public static SoundEffect Footsteps { get; private set; }
        public static SoundEffect Shooting { get; private set; }
        public static SoundEffect Upgrade { get; private set; }
        public static SoundEffect Button { get; private set; }
        public static SoundEffect TakingDamage { get; private set; }

        public static SoundEffect BossDeath { get; private set; }
        public static SoundEffect BossSpawn { get; private set; }
        public static SoundEffect EnemyDeath { get; private set; }

        public static SoundEffect EarthAttack { get; private set; }
        public static SoundEffect FireAttack { get; private set; }
        public static SoundEffect MetalAttack { get; private set; }
        public static SoundEffect WaterAttack { get; private set; }
        public static SoundEffect WoodAttack { get; private set; }

        public static SoundEffect Door { get; private set; }
        public static SoundEffect StartingScreen { get; private set; }

        public static void Load(ContentManager Content)
        {
            SoundEffect.MasterVolume = 0.4f;
            AdamDeath = Content.Load<SoundEffect>("Sounds\\Adam Death");
            Shooting = Content.Load<SoundEffect>("Sounds\\Shooting");
            Footsteps = Content.Load<SoundEffect>("Sounds\\Footsteps");
            Upgrade = Content.Load<SoundEffect>("Sounds\\Upgrade");
            Button = Content.Load<SoundEffect>("Sounds\\Button");
            TakingDamage = Content.Load<SoundEffect>("Sounds\\Taking Damage");

            BossDeath = Content.Load<SoundEffect>("Sounds\\Boss Death");
            BossSpawn = Content.Load<SoundEffect>("Sounds\\Boss Spawn");
            EnemyDeath = Content.Load<SoundEffect>("Sounds\\Enemy Death");

            EarthAttack = Content.Load<SoundEffect>("Sounds\\Earth Attack");
            FireAttack = Content.Load<SoundEffect>("Sounds\\Fire Attack");
            MetalAttack = Content.Load<SoundEffect>("Sounds\\Metal Attack");
            WaterAttack = Content.Load<SoundEffect>("Sounds\\Water Attack");
            WoodAttack = Content.Load<SoundEffect>("Sounds\\Wood Attack");

            Door = Content.Load<SoundEffect>("Sounds\\Door");
            StartingScreen = Content.Load<SoundEffect>("Sounds\\Starting Screen");

            SoundEffectInstance StartingScreenInstance = StartingScreen.CreateInstance();
            StartingScreenInstance.Play();

        }
    }
}

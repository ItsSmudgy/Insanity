﻿using Fandomonium.Common.Systems.ParticleSystem;
using Fandomonium.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Fandomonium.Content.NPCs.SpiritOfLight.Projectiles
{
    public class RadiantGemDeathrayTelegraph : ModProjectile
    {
        public override string Texture => "Fandomonium/Assets/Textures/BasicDeathrayTextureFront";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Insurgency Ray");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            behindProjectiles.Add(index);
        }

        public override bool? CanDamage() { return false; }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            Projectile Parent = Main.projectile[(int)Projectile.ai[1]];
            if (!Parent.active || Parent.type != ModContent.ProjectileType<RadiantGem>())
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = Parent.Center;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            float num801 = 0.45f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 60)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / 60) * 10f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num804 = Parent.localAI[1] - 1.57079637f + Projectile.ai[0];
            Projectile.rotation = num804;
            num804 += 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPos = vector79;
                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(7f, 14f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.35f, 0.85f), Main.rand.Next(100, 120), false, true));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D18 = Mod.Assets.Request<Texture2D>("Assets/Textures/BasicDeathrayTextureFront").Value;
            Texture2D texture2D19 = Mod.Assets.Request<Texture2D>("Assets/Textures/BasicDeathrayTextureMiddle").Value;
            Texture2D texture2D20 = Mod.Assets.Request<Texture2D>("Assets/Textures/BasicDeathrayTextureEnd").Value;
            float num207 = Projectile.localAI[1];
            Microsoft.Xna.Framework.Color color40 = Color.Lerp(Color.White, Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f)), 0.95f);
            color40 = Color.Lerp(color40, Color.Transparent, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D18, Projectile.Center - Main.screenPosition, null, color40, Projectile.rotation, texture2D18.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num207 -= (float)(texture2D18.Height / 2 + texture2D20.Height) * Projectile.scale;
            Vector2 center3 = Projectile.Center;
            center3 += Projectile.velocity * Projectile.scale * texture2D18.Height / 2f;
            if (num207 > 0f)
            {
                float num208 = 0f;
                Microsoft.Xna.Framework.Rectangle value12 = new Microsoft.Xna.Framework.Rectangle(0, 16 * (Projectile.timeLeft / 3 % 5), texture2D19.Width, 16);
                while (num208 + 1f < num207)
                {
                    if (num207 - num208 < (float)value12.Height)
                    {
                        value12.Height = (int)(num207 - num208);
                    }
                    Main.EntitySpriteDraw(texture2D19, center3 - Main.screenPosition, value12, color40, Projectile.rotation, new Vector2(value12.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    num208 += (float)value12.Height * Projectile.scale;
                    center3 += Projectile.velocity * value12.Height * Projectile.scale;
                    value12.Y += 16;
                    if (value12.Y + value12.Height > texture2D19.Height)
                    {
                        value12.Y = 0;
                    }
                }
            }
            Main.EntitySpriteDraw(texture2D20, center3 - Main.screenPosition, null, color40, Projectile.rotation, texture2D20.Frame().Top(), Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}

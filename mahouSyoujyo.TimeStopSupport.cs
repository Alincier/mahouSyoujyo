using mahouSyoujyo.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using ReLogic.Content;
using mahouSyoujyo.Common.Systems;
using mahouSyoujyo.Content.Projectiles;
using mahouSyoujyo.Content.Buffs;
using Terraria.Chat;
using Terraria.Localization;
using mahouSyoujyo.Content.NPCs.Critters;
using XPT.Core.Audio.MP3Sharp.Decoding;
using System.IO;
using Humanizer;
namespace mahouSyoujyo
{

    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public partial class mahouSyoujyo: Mod
    {
        public static bool stoptime = false;

        private void On_NPC_Transform(On_NPC.orig_Transform orig, NPC self, int newType)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self , newType);
            }
            catch
            {


            }
        }
        private void On_NPC_UpdateNPC_UpdateTrails(On_NPC.orig_UpdateNPC_UpdateTrails orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch 
            {

                
            }
            
        }

        private void On_NPC_CheckLifeRegen(On_NPC.orig_CheckLifeRegen orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            } 
            catch { }

        }

        private void On_NPC_SubAI_HandleTemporaryCatchableNPCPlayerInvulnerability(On_NPC.orig_SubAI_HandleTemporaryCatchableNPCPlayerInvulnerability orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }
        }

        private void On_NPC_UpdateNPC_BloodMoonTransformations(On_NPC.orig_UpdateNPC_BloodMoonTransformations orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }
        }

        private void On_NPC_UpdateNPC_UpdateGravity(On_NPC.orig_UpdateNPC_UpdateGravity orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }
        }

        private void On_NPC_UpdateNPC_CritterSounds(On_NPC.orig_UpdateNPC_CritterSounds orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }
        }


        private void On_NPC_UpdateAltTexture(On_NPC.orig_UpdateAltTexture orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }
        }

        private void On_NPC_AI(On_NPC.orig_AI orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }
        }

        private void On_NPC_FindFrame(On_NPC.orig_FindFrame orig, NPC self)
        {
            try
            {
                if (!self.active) return;
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig(self);
            }
            catch { }

        }

        private void On_NPC_SpawnNPC(On_NPC.orig_SpawnNPC orig)
        {
            try
            {
                if (TimeStopSystem.TimeStopping)
                {
                    return;
                }
                orig();
            }
            catch { }
        }
        private void On_Dust_UpdateDust(On_Dust.orig_UpdateDust orig)
        {
            if (TimeStopSystem.TimeStopping) return;
            orig();

        }
        private void On_Rain_Update(On_Rain.orig_Update orig, Rain self)
        {
            if (TimeStopSystem.TimeStopping) return;
            if (self.active) orig(self);
        }

        private void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            try
            {
                if (self.active && TimeStopSystem.TimeStopping)
                {
                    self.GetGlobalNPC<GlobalStopNPC>().oldCenter = self.Center;
                    self.GetGlobalNPC<GlobalStopNPC>().oldVelocity = self.velocity;
                }
                if (self.active) orig(self, i);
                if (self.active && TimeStopSystem.TimeStopping)
                {
                    if (self.GetGlobalNPC<GlobalStopNPC>().oldCenter == self.Center && self.GetGlobalNPC<GlobalStopNPC>().oldVelocity == self.oldVelocity) return;
                    self.Center = self.GetGlobalNPC<GlobalStopNPC>().oldCenter;
                    self.velocity = self.GetGlobalNPC<GlobalStopNPC>().oldVelocity;
                }
            }
            catch { }
            
        }




        //旧版NPC时停的处理方法
        private void On_NPC_UpdateNPC2(On_NPC.orig_UpdateNPC orig, NPC self, int i, int j = 0)
        {
            
            if (TimeStopSystem.TimeStopping) 
            {
                if (Main.netMode == NetmodeID.Server) ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(self.active.ToString()+" "+self.life.ToString()), Color.White);
                /*if (self.active) 
                for (i=0; i<256; i++)
                {
                    if (self.immune[i]>0) self.immune[i]--;
                    else self.immune[i] = 0;
                }*/
                //self.CheckActive();
                //self.checkDead();
                //if (Main.netMode != NetmodeID.Server)
                {
                    //if (self.life <= 0 || !self.active)
                    {
                        //self.active = false;
                        //NetMessage.TrySendData(MessageID.SyncNPC, -1, -1, null, self.whoAmI);
                        //self.netUpdate = true;

                        self.UpdateAltTexture();
                        if (self.life <= 0)
                        {
                            self.active = false;
                            {
                                if (!self.active)
                                    self.netUpdate = true;

                                if (Main.netMode != 2)
                                    return;



                                if (self.townNPC || self.aiStyle == 16)
                                    self.netSpam = 0;

                                if (self.netUpdate2)
                                    self.netUpdate = true;

                                if (!self.active)
                                    self.netSpam = 0;

                                if (self.netUpdate)
                                {
                                    if (self.boss)
                                    {
                                        _ = self.oldPosition - self.position;
                                        if (self.netSpam <= 15)
                                        {
                                            self.netSpam += 5;
                                            NetMessage.SendData(23, -1, -1, null, i);
                                            self.netUpdate2 = false;
                                        }
                                        else
                                        {
                                            self.netUpdate2 = true;
                                        }
                                    }
                                    else if (self.netSpam <= 90)
                                    {
                                        self.netSpam += 30;
                                        NetMessage.SendData(23, -1, -1, null, i);
                                        self.netUpdate2 = false;
                                    }
                                    else
                                    {
                                        self.netUpdate2 = true;
                                    }
                                }

                                if (self.netSpam > 0)
                                    self.netSpam--;

                                if (self.active && self.townNPC && NPC.TypeToDefaultHeadIndex(self.type) > 0)
                                {
                                    if (self.homeless != self.oldHomeless || self.homeTileX != self.oldHomeTileX || self.homeTileY != self.oldHomeTileY)
                                    {
                                        byte householdStatus = WorldGen.TownManager.GetHouseholdStatus(self);
                                        NetMessage.SendData(60, -1, -1, null, i, Main.npc[i].homeTileX, Main.npc[i].homeTileY, (int)householdStatus);
                                    }

                                    self.oldHomeless = self.homeless;
                                    self.oldHomeTileX = self.homeTileX;
                                    self.oldHomeTileY = self.homeTileY;
                                }
                            }
                            self.netUpdate = false;
                            self.justHit = false;
                            return;
                        }
                        self.oldTarget = self.target;
                        self.oldDirection = self.direction;
                        self.oldDirectionY = self.directionY;

                        self.FindFrame();
                        {
                            if (!self.active)
                                self.netUpdate = true;

                            if (Main.netMode != 2)
                                return;



                            if (self.townNPC || self.aiStyle == 16)
                                self.netSpam = 0;

                            if (self.netUpdate2)
                                self.netUpdate = true;

                            if (!self.active)
                                self.netSpam = 0;

                            if (self.netUpdate)
                            {
                                if (self.boss)
                                {
                                    _ = self.oldPosition - self.position;
                                    if (self.netSpam <= 15)
                                    {
                                        self.netSpam += 5;
                                        NetMessage.SendData(23, -1, -1, null, i);
                                        self.netUpdate2 = false;
                                    }
                                    else
                                    {
                                        self.netUpdate2 = true;
                                    }
                                }
                                else if (self.netSpam <= 90)
                                {
                                    self.netSpam += 30;
                                    NetMessage.SendData(23, -1, -1, null, i);
                                    self.netUpdate2 = false;
                                }
                                else
                                {
                                    self.netUpdate2 = true;
                                }
                            }

                            if (self.netSpam > 0)
                                self.netSpam--;

                            if (self.active && self.townNPC && NPC.TypeToDefaultHeadIndex(self.type) > 0)
                            {
                                if (self.homeless != self.oldHomeless || self.homeTileX != self.oldHomeTileX || self.homeTileY != self.oldHomeTileY)
                                {
                                    byte householdStatus = WorldGen.TownManager.GetHouseholdStatus(self);
                                    NetMessage.SendData(60, -1, -1, null, i, Main.npc[i].homeTileX, Main.npc[i].homeTileY, (int)householdStatus);
                                }

                                self.oldHomeless = self.homeless;
                                self.oldHomeTileX = self.homeTileX;
                                self.oldHomeTileY = self.homeTileY;
                            }
                        }
                        self.CheckActive();
                        self.netUpdate = false;
                        self.justHit = false;




                    }
                    /*else

                    if (self.realLife >= 0 )
                    {
                        if (Main.npc[self.realLife].life <= 0|| !Main.npc[self.realLife].active)
                        {
                            Main.npc[self.realLife].active = false;
                            NetMessage.TrySendData(23, -1, -1, null, self.realLife);
                            self.netUpdate = true;
                        }
                            
                    }*/
                }
                if (Main.netMode == NetmodeID.Server) ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(self.active.ToString()+" "+self.life.ToString()), Color.White);
                return;
            }
            if (Main.netMode == NetmodeID.Server && self.ModNPC is Critter_QB ) ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(self.active.ToString()+" "+self.life.ToString()), Color.White);
            if (self.active) orig(self,i);
        }

        private void On_Main_UpdateTime(On_Main.orig_UpdateTime orig)
        {
            if (TimeStopSystem.TimeStopping) return;
            orig(); 
            //throw new NotImplementedException();
        }
        private void On_Player_Update(On_Player.orig_Update orig, Player self, int i)
        {
            try
            {
                if (!self.active) return;
                bool isBind = self.GetModPlayer<TimeStop>().bind;
                if (self.GetModPlayer<TimeStop>().spawnTime > 0) self.GetModPlayer<TimeStop>().spawnTime--;
                if (TimeStopSystem.TimeStopping && !isBind)
                {
                    if (!self.dead && self.statLife > 0 && self.GetModPlayer<TimeStop>().spawnTime <= 0) return;
                }

                if (self.active) orig(self, i);
            }
            catch 
            {

            }
            //throw new NotImplementedException();
        }

        //public override 
        private void On_Projectile_Update(On_Projectile.orig_Update orig, Projectile self, int i)
        {
            try 
            {  if (!self.active) return;
                //时间导弹特供
                if (self.active && self.type == ModContent.ProjectileType<TimeMissile>())
                {
                    TimeMissile missile = (TimeMissile)self.ModProjectile;
                    missile.runtime++;
                    int magic = 0;
                    if (Main.player[missile.Projectile.owner].HasBuff(ModContent.BuffType<MagicGirlPover>())) magic = 1;
                    if (missile.runtime < 30) missile.stage = 0;
                    else if (missile.runtime < 40) missile.stage = 1;
                    else if (missile.runtime < 50) missile.stage = 2;
                    else if (missile.runtime < 60) missile.stage = 3;
                    else if (missile.runtime < 90) missile.stage = 4;
                    else missile.stage = 4+magic;
                }
                bool specialtype = false;
                if (self.aiStyle == ProjAIStyleID.Flail || self.aiStyle == ProjAIStyleID.Yoyo ||
                    self.aiStyle == ProjAIStyleID.Whip || self.aiStyle == ProjAIStyleID.WireKite ||
                    self.aiStyle == ProjAIStyleID.Pet || self.aiStyle == ProjAIStyleID.FloatBehindPet ||
                    self.aiStyle == ProjAIStyleID.FloatInFrontPet || self.aiStyle == ProjAIStyleID.FloatingFollow ||
                    self.aiStyle == ProjAIStyleID.WormPet ||
                    self.aiStyle == ProjAIStyleID.MagicMissile || self.aiStyle == ProjAIStyleID.Boomerang ||
                    self.aiStyle == ProjAIStyleID.Bubble || self.aiStyle == ProjAIStyleID.WaterJet ||
                    self.aiStyle == ProjAIStyleID.Beam || self.aiStyle == ProjAIStyleID.ThickLaser ||
                    self.aiStyle == ProjAIStyleID.ScutlixLaser || self.aiStyle == ProjAIStyleID.Drill ||
                    self.aiStyle == ProjAIStyleID.FallingStar ||self.aiStyle == ProjAIStyleID.Spear ||
                    self.aiStyle == ProjAIStyleID.ShortSword || self.aiStyle == ProjAIStyleID.SmallFlying ||
                    self.aiStyle == ProjAIStyleID.FlowerPetal || self.aiStyle == ProjAIStyleID.TitaniumShard ||
                    self.aiStyle == ProjAIStyleID.CrystalLeaf || self.aiStyle == ProjAIStyleID.CrystalLeafShot ||
                    self.aiStyle == ProjAIStyleID.Harpoon || self.aiStyle == ProjAIStyleID.HeldProjectile ||
                    self.aiStyle == ProjAIStyleID.NightsEdge || self.aiStyle == ProjAIStyleID.TrueNightsEdge ||
                    self.aiStyle == ProjAIStyleID.PrincessWeapon

                    )
                    specialtype = true;
                bool held = false;
                for (int j = 0; j<256; j++)
                    if (Main.player[j].heldProj==self.whoAmI)
                    { held = true; break; }

                if (TimeStopSystem.TimeStopping && ((Array.IndexOf(TimeStop.immuneProjectile, self.type)<0 && !held && !specialtype && !self.hide) || self.hostile))
                {
                    GlobalStopProjectile sp;
                    if (self.active)
                    {
                        if (!Main.projPet[self.type] && !self.minion && !self.sentry && !Main.projHook[self.type])
                        {
                            sp = self.GetGlobalProjectile<GlobalStopProjectile>();
                            if (sp.runningFrameTime >= 5)
                                return;
                            sp.runningFrameTime++;
                        }
                        //if (sp.runningFrameTime > sp.runningFrame /2)
                        //{
                        //    sp.runningFrameTime = 0;
                        //    sp.runningFrame++;
                        //}
                        //else return;

                    }
                }

                if (self.active)
                {
                    bool isBind = (self.owner != 255 && Main.player[self.owner].active) ? Main.player[self.owner].GetModPlayer<TimeStop>().bind : false;
                    if (TimeStopSystem.TimeStopping && !isBind) return;
                    orig(self, i);
                }
            }
            catch { }
            
        }



    }
}

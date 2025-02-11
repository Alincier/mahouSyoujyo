using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using mahouSyoujyo.Content.Items.RangedWeapon;
using Steamworks;
using Terraria.Localization;
using System.Transactions;
using Terraria.DataStructures;
using Terraria.Audio;
using mahouSyoujyo.Content.Items;
using mahouSyoujyo.Globals;

namespace mahouSyoujyo.Content.Buffs
{
    public class MagicGirlPover : ModBuff//继承modbuff类
    {
        //public override string LocalizationCategory =>"Buffs";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;//为true时就是debuff
            Main.buffNoSave[Type] = false;//为true时退出世界时buff消失
            Main.buffNoTimeDisplay[Type] = true;//为true时不显示剩余时间
            //以下为debuff不可被护士去除
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //以下为专家模式Debuff持续时间是否延长
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;//无限持续
            //死亡不清除
            Main.persistentBuff[Type] = true;
            // 决定这个buff是不是照明宠物的buff，宠物和召唤物的时候会用到
            Main.lightPet[Type] = false;
            // 决定这个buff是不是一个装饰性宠物，用来判定的，比如消除buff的时候不会消除它
            Main.vanityPet[Type] = false;
            
        }
        public override bool RightClick(int buffIndex)
        {
            return false;
        }
        public override void Update(Player player, ref int buffIndex)//这个函数是当玩家获得BUFF时每帧执行一次，玩家大多数属性每帧重置
        {
            player.buffTime[buffIndex]=10;

            // 每帧1/200的概率让玩家散发粉色粒子
            if (Main.rand.NextBool(200))
            {
                Dust d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.PinkFairy);
                d.velocity *= 0.3f;
            }
            player.lifeRegen += 5;//生命回复+5
            player.moveSpeed += 0.1f;//10%移速加成
            //player.accRunSpeed += 0.5f;//给予0.5加速度
            player.statLifeMax2 += player.statLifeMax / 10;//最大生命+10%，注意，是lifemax2，lifemax是存档生命上限（吃生命水晶的那种）
            player.statManaMax2 += player.statManaMax / 10;//同理魔法值也是如此
            player.statDefense += 10;//防御力+10
            player.GetDamage(DamageClass.Generic) += 0.1f;//攻击力倍率可以加算也可以乘算，但是乘算容易数值膨胀
            player.GetCritChance(DamageClass.Generic) += 0.1f;//暴击率同理
            player.maxMinions += 1;//召唤上限+1
            player.endurance += 0.1f;//伤害减免+10%
            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;//近战攻速+10%
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (Main.netMode !=  NetmodeID.Server)
            {
                if (Main.LocalPlayer.active)
                {
                    if (Main.LocalPlayer.magic().relief)
                        tip+="\n"+Language.GetText("Mods.mahouSyoujyo.Buffs.MagicGirlPover.relief");
                    else tip+="\n"+Language.GetText("Mods.mahouSyoujyo.Buffs.MagicGirlPover.notrelief");
                }
            }
            base.ModifyBuffText(ref buffName, ref tip, ref rare);
        }
    }
}

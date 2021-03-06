﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using CM = KurisuNidalee.Combat;
using ES = KurisuNidalee.Essentials;

namespace KurisuNidalee
{
    internal class KurisuNidalee
    {
        internal static Menu Root;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Obj_AI_Hero Player = ObjectManager.Player;

        internal KurisuNidalee()
        {
            // Console.WriteLine("Nidalee Loaded!");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        internal static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Nidalee")
                return;

            Root = new Menu("Kurisu's Nidalee", "nidalee", true);

            var humenu = new Menu("Human", "humenu");

            var ndhq = new Menu("(Q)  Javelin", "ndhq");
            ndhq.AddItem(new MenuItem("ndhqcheck", "Check Hitchance")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqch", "-> Min Hitchance"))
                .SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3));
            ndhq.AddItem(new MenuItem("ndhqco", "Enable in Combo")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqha", "Enable in Harass")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqjg", "Enable in Jungle")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqwc", "Enable in WaveClear")).SetValue(false);
            ndhq.AddItem(new MenuItem("ndhqimm", "-> Auto (Q) Immobile Targets")).SetValue(true);
            ndhq.AddItem(new MenuItem("ndhqgap", "-> Auto (Q) Enemy Gapclosers")).SetValue(true);
            humenu.AddSubMenu(ndhq);

            var ndhw = new Menu("(W) Bushwack", "ndhw");
            ndhw.AddItem(new MenuItem("ndhwco", "Enable in Combo")).SetValue(true);
            ndhw.AddItem(new MenuItem("ndhwjg", "Enable in Jungle")).SetValue(true);
            ndhw.AddItem(new MenuItem("ndhwwc", "Enable in WaveClear")).SetValue(false);
            ndhw.AddItem(new MenuItem("ndhwforce", "Location")).SetValue(new StringList(new[] { "Prediction", "Behind Target" }));
            ndhw.AddItem(new MenuItem("ndhwimm", "-> Auto (W) Immobile Targets")).SetValue(true);
            humenu.AddSubMenu(ndhw);

            var ndhe = new Menu("(E)  Primal Surge", "ndhe");
            ndhe.AddItem(new MenuItem("ndheon", "Enable Healing")).SetValue(true);
            ndhe.AddItem(new MenuItem("ndhemana", "- > Minumum Mana")).SetValue(new Slider(55, 1));
            ndhe.AddItem(new MenuItem("ndhesw", "Switch Forms")).SetValue(false);

            foreach (var hero in HeroManager.Allies)
            {
                ndhe.AddItem(new MenuItem("x" + hero.ChampionName, "Heal on " + hero.ChampionName)).SetValue(hero.IsMe);
                ndhe.AddItem(new MenuItem("z" + hero.ChampionName, hero.ChampionName + " below Pct% ")).SetValue(new Slider(66, 1, 99));
            }

            humenu.AddSubMenu(ndhe);

            var ndhr = new Menu("(R) Aspect of the Cougar", "ndhr");
            ndhr.AddItem(new MenuItem("ndhrwh", "Switch When")).SetValue(new StringList(new[] { "Auto", "Hunted Only", "Always" }));
            ndhr.AddItem(new MenuItem("ndhrco", "Enable in Combo")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrcreq", "- > Require Swipe/Takedown")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrha", "Enable in Harass")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrjg", "Enable in Jungle")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrjreq", "- > Require Swipe/Takedown")).SetValue(true);
            ndhr.AddItem(new MenuItem("ndhrwc", "Enable in WaveClear")).SetValue(false);
            ndhr.AddItem(new MenuItem("ndhrgap", "-> Auto (R) Enemy Gapclosers")).SetValue(true);
            humenu.AddSubMenu(ndhr);

            Root.AddSubMenu(humenu);

            var comenu = new Menu("Cougar", "comenu");

            var ndcq = new Menu("(Q) Takedown", "ndcq");
            ndcq.AddItem(new MenuItem("ndcqco", "Enable in Combo")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqha", "Enable in Harass")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqjg", "Enable in Jungle")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqwc", "Enable in WaveClear")).SetValue(true);
            ndcq.AddItem(new MenuItem("ndcqgap", "-> Auto (Q) Enemy Gapclosers")).SetValue(true);
            comenu.AddSubMenu(ndcq);

            var ndcw = new Menu("(W) Pounce", "ndcw");
            ndcw.AddItem(new MenuItem("ndcwcheck", "Check Hitchance")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwch", "-> Min Hitchance"))
                .SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2));
            ndcw.AddItem(new MenuItem("ndcwco", "Enable in Combo")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwhunt", "-> Force Pounce if Hunted")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwgap", "-> Gapclose Pounce")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwdistco", "-> Pounce Only if > AARange")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwjg", "Enable in Jungle")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwwc", "Enable in WaveClear")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwdistwc", "-> Pounce Only if > AARange")).SetValue(false);
            ndcw.AddItem(new MenuItem("ndcwene", "-> Dont Pounce into Enemies")).SetValue(true);
            ndcw.AddItem(new MenuItem("ndcwtow", "-> Dont Pounce into Turret")).SetValue(true);
            comenu.AddSubMenu(ndcw);

            var ndce = new Menu("(E) Swipe", "ndce");
            ndce.AddItem(new MenuItem("ndcecheck", "Check Hitchance")).SetValue(false);
            ndce.AddItem(new MenuItem("ndcech", "-> Min Hitchance"))
                .SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2));

            ndce.AddItem(new MenuItem("ndceco", "Enable in Combo")).SetValue(true);
            ndce.AddItem(new MenuItem("ndceha", "Enable in Harass")).SetValue(true);
            ndce.AddItem(new MenuItem("ndcejg", "Enable in Jungle")).SetValue(true);
            ndce.AddItem(new MenuItem("ndcewc", "Enable in WaveClear")).SetValue(true);
            ndce.AddItem(new MenuItem("ndcenum", "-> Minimum Minions Hit")).SetValue(new Slider(3, 1, 5));
            ndce.AddItem(new MenuItem("ndcegap", "-> Auto (E) Enemy Gapclosers")).SetValue(true);
            comenu.AddSubMenu(ndce);

            var ndcr = new Menu("(R) Aspect of the Cougar", "ndcr");
            ndcr.AddItem(new MenuItem("ndcrwh", "Switch When")).SetValue(new StringList(new[] { "Auto", "Q Ready", "Out of Range" }));
            ndcr.AddItem(new MenuItem("ndcrco", "Enable in Combo")).SetValue(true);
            ndcr.AddItem(new MenuItem("ndcrha", "Enable in Harass")).SetValue(true);
            ndcr.AddItem(new MenuItem("ndcrjg", "Enable in Jungle")).SetValue(true);
            ndcr.AddItem(new MenuItem("ndcrwc", "Enable in WaveClear")).SetValue(false);

            comenu.AddSubMenu(ndcr);

            Root.AddSubMenu(comenu);

            var orbm = new Menu("Orbwalker", "orbm");

            var tsmenu = new Menu("Target Selector", "tsmenu");
            TargetSelector.AddToMenu(tsmenu);
            orbm.AddSubMenu(tsmenu);

            Orbwalker = new Orbwalking.Orbwalker(orbm);
            Root.AddSubMenu(orbm);

            var jmenu = new Menu("Other/Misc", "jmenu");
            jmenu.AddItem(new MenuItem("prediction", "-> P")).SetValue(new StringList(new[] { "Common", "SPrediction" }));
            jmenu.AddItem(new MenuItem("jgsmite", "Enable Smite")).SetValue(true);
            jmenu.AddItem(new MenuItem("jgsmitetd", "Takedown + Smite")).SetValue(true);
            jmenu.AddItem(new MenuItem("jgsmiteep", "-> Smite Epic")).SetValue(true);
            jmenu.AddItem(new MenuItem("jgsmitebg", "-> Smite Large")).SetValue(true);
            jmenu.AddItem(new MenuItem("jgsmitesm", "-> Smite Small")).SetValue(false);
            jmenu.AddItem(new MenuItem("jgsmitehe", "-> Smite On Hero")).SetValue(true);
            jmenu.AddItem(new MenuItem("jgsticky", "Sticky Jungling")).SetValue(true);
            jmenu.AddItem(new MenuItem("jgaacount", "Require 2 AA")).SetValue(true);
            jmenu.AddItem(new MenuItem("exaa", "Experimental AA Resets")).SetValue(false);
            Root.AddSubMenu(jmenu);

            Root.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapclose;
        }

        internal static void OnEnemyGapclose(ActiveGapcloser gapcloser)
        {
            var attacker = gapcloser.Sender;
            if (attacker.IsValidTarget(275f))
            {
                if (ES.CatForm())
                {
                    CM.CastJavelin(attacker, "gap");
                    CM.SwitchForm(attacker, "gap");
                }

                else
                {
                    CM.CastSwipe(attacker, "gap");
                }
            }
        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Jungle();
                    WaveClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            // auto bushwack on immobile
            if (Root.Item("ndhwimm").GetValue<bool>() && !ES.CatForm())
                foreach (var unit in HeroManager.Enemies.Where(h => h.IsValidTarget(ES.Spells["Bushwack"].Range) && ES.Immobile(h)))
                    ES.Spells["Bushwack"].Cast(unit);

            // auto javelin on immobile
            if (Root.Item("ndhqimm").GetValue<bool>() && !ES.CatForm())
                foreach (var unit in HeroManager.Enemies.Where(h => h.IsValidTarget(ES.Spells["Javelin"].Range) && ES.Immobile(h)))
                    ES.Spells["Javelin"].Cast(unit);

            // auto heal on ally hero
            if (Root.Item("ndheon").GetValue<bool>() && ES.SpellTimer["Primalsurge"].IsReady())
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None ||
                    Player.Spellbook.IsChanneling || Player.IsRecalling())
                    return;

                if (Player.Mana/Player.MaxMana*100 < 
                    Root.Item("ndhemana").GetValue<Slider>().Value)
                    return;

                foreach (
                    var hero in
                        HeroManager.Allies.Where(
                            h => Root.Item("x" + h.ChampionName).GetValue<bool>() &&
                                 h.IsValidTarget(ES.Spells["Primalsurge"].Range, false) &&
                                 h.Health/h.MaxHealth*100 < Root.Item("z" + h.ChampionName).GetValue<Slider>().Value))
                {
                    if (ES.CatForm() == false)
                        ES.Spells["Primalsurge"].CastOnUnit(hero);

                    if (ES.CatForm() && Root.Item("ndhesw").GetValue<bool>() && ES.Spells["Aspect"].IsReady())
                        ES.Spells["Aspect"].Cast();
                }
            }
        }

        internal static void Combo()
        {
            CM.CastJavelin(TargetSelector.GetTarget(ES.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "co");
            CM.CastBushwack(TargetSelector.GetTarget(ES.Spells["Bushwack"].Range, TargetSelector.DamageType.Magical), "co");
            CM.CastTakedown(TargetSelector.GetTarget(ES.Spells["Takedown"].Range, TargetSelector.DamageType.Magical), "co");
            CM.CastPounce(TargetSelector.GetTarget(ES.Spells["ExPounce"].Range, TargetSelector.DamageType.Magical), "co");
            CM.CastSwipe(TargetSelector.GetTarget(ES.Spells["Swipe"].Range, TargetSelector.DamageType.Magical), "co");
            CM.SwitchForm(TargetSelector.GetTarget(ES.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "co");
        }

        internal static void Harass()
        {
            CM.CastJavelin(TargetSelector.GetTarget(ES.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "ha");            
            CM.CastTakedown(TargetSelector.GetTarget(ES.Spells["Takedown"].Range, TargetSelector.DamageType.Magical), "ha");
            CM.CastSwipe(TargetSelector.GetTarget(ES.Spells["Swipe"].Range, TargetSelector.DamageType.Magical), "ha");
            CM.SwitchForm(TargetSelector.GetTarget(ES.Spells["Javelin"].Range, TargetSelector.DamageType.Magical), "ha");
        }

        internal static void Jungle()
        {
            foreach (
                var minion in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => ES.MinionList.Any(y => x.Name.StartsWith(y) && !x.Name.Contains("Mini"))))
            {
                if (minion.IsValidTarget(850))
                {
                    CM.CastJavelin(minion, "jg");
                    CM.CastBushwack(minion, "jg");
                    CM.CastTakedown(minion, "jg");
                    CM.CastPounce(minion, "jg");
                    CM.CastSwipe(minion, "jg");
                    CM.SwitchForm(minion, "jg");
                    return;
                }
            }

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => !x.Name.StartsWith("Minion")))
            {
                if (minion.IsValidTarget(850))
                {
                    CM.CastJavelin(minion, "jg");
                    CM.CastBushwack(minion, "jg");
                    CM.CastTakedown(minion, "jg");
                    CM.CastPounce(minion, "jg");
                    CM.CastSwipe(minion, "jg");
                    CM.SwitchForm(minion, "jg");
                }
            }
        }

        internal static void WaveClear()
        {
            foreach (
                var minion in
                    ES.MinionCache.Values.Where(
                        x => x.IsMinion && x.IsValid && x.Distance(Player.ServerPosition) <= 850))
            {
                CM.CastJavelin(minion, "wc");
                CM.CastBushwack(minion, "wc");
                CM.CastTakedown(minion, "wc");
                CM.CastPounce(minion, "wc");
                CM.CastSwipe(minion, "wc");
                CM.SwitchForm(minion, "wc");
            }
        }
    }
}

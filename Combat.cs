using LeagueSharp;
using LeagueSharp.Common;
using ES = KurisuNidalee.Essentials;
using KN = KurisuNidalee.KurisuNidalee;

namespace KurisuNidalee
{
    internal class Combat
    {
        // Human Q Logic
        internal static void CastJavelin(Obj_AI_Base target, string mode)
        {
            // if not harass mode ignore mana check
            if (!ES.CatForm() && (mode != "ha" || ES.Player.ManaPercent > 65))
            {
                if (ES.SpellTimer["Javelin"].IsReady() && KN.Root.Item("ndhq" + mode).GetValue<bool>())
                {
                    if (target.IsValidTarget(ES.Spells["Javelin"].Range))
                    {
                        // try prediction on champion
                        if (target.IsChampion() && KN.Root.Item("ndhqcheck").GetValue<bool>())
                        {
                            if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 0)
                            {
                                ES.Spells["Javelin"].CastIfHitchanceEquals(target,
                                    (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3));
                            }

                            if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 1)
                            {
                                //SPrediction.Prediction.SPredictionCast(ES.Spells["Javelin"], (Obj_AI_Hero) target,
                                //    (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3));
                            }
                        }

                        else
                            ES.Spells["Javelin"].Cast(target);
                    }
                }
            }
        }

        // Human W Logic
        internal static void CastBushwack(Obj_AI_Base target, string mode)
        {           
            // if not harass mode ignore mana check
            if (!ES.CatForm() && (mode != "ha" || ES.Player.ManaPercent > 65))
            {
                if (ES.SpellTimer["Bushwack"].IsReady() && KN.Root.Item("ndhw" + mode).GetValue<bool>())
                {
                    // try bushwack prediction
                    if (KN.Root.Item("ndhwforce").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (target.IsValidTarget(ES.Spells["Bushwack"].Range))
                        {
                            if (target.IsChampion())
                            {
                                if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 0)
                                    ES.Spells["Bushwack"].CastIfHitchanceEquals(target, HitChance.VeryHigh);
                                //else
                                //    SPrediction.Prediction.SPredictionCast(ES.Spells["Bushwack"], (Obj_AI_Hero) target, HitChance.High);
                            }
                            else
                                ES.Spells["Bushwack"].Cast(target.ServerPosition);
                        }
                    }

                    // try bushwack behind target
                    if (KN.Root.Item("ndhwforce").GetValue<StringList>().SelectedIndex == 1)
                    {
                        if (target.IsValidTarget(ES.Spells["Bushwack"].Range))
                        {
                            // todo: add adjust-able range
                            ES.Spells["Bushwack"].Cast(
                                target.IsChampion()
                                    ? Prediction.GetPrediction(target, 0.25f)
                                        .UnitPosition.Extend(ES.Player.ServerPosition, -75f)
                                    : target.ServerPosition.Extend(ES.Player.ServerPosition, -75f));
                        }
                    }
                }
            }
        }


        // Cougar Q Logic
        internal static void CastTakedown(Obj_AI_Base target, string mode)
        {
            if (ES.CatForm())
            {
                if (ES.SpellTimer["Takedown"].IsReady() && KN.Root.Item("ndcq" + mode).GetValue<bool>())
                {
                    // temp logic to prevent takdown cast before pounce
                    if (!ES.SpellTimer["Swipe"].IsReady() || ES.NotLearned(ES.Spells["Swipe"]) || !KN.Root.Item("ndce" + mode).GetValue<bool>())
                    {
                        if (target.IsValidTarget(ES.Player.AttackRange + ES.Spells["Takedown"].Range))
                        {
                            ES.Spells["Takedown"].CastOnUnit(ES.Player);

                            // force attack order on target (smoother)
                            if (ES.Player.HasBuff("Takdown"))
                                ES.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                }
            }
        }


        // Cougar W Logic
        internal static void CastPounce(Obj_AI_Base target, string mode)
        {
            if (!ES.CatForm())
                return;

            if (ES.SpellTimer["Pounce"].IsReady() && KN.Root.Item("ndcw" + mode).GetValue<bool>())
            {
                if (!target.IsValidTarget(ES.Spells["ExPounce"].Range))
                    return;

                // if target is hunted in 750 range
                if (target.IsHunted())
                {
                    var radius = ES.Player.AttackRange + ES.Player.Distance(ES.Player.BBox.Minimum) + 1;

                    // force pounce if menu item enabled
                    if (target.IsHunted() && KN.Root.Item("ndcwhunt").GetValue<bool>() ||

                        // or of target is greater than my attack range
                        target.Distance(ES.Player.ServerPosition) > radius ||

                        // or is jungling or waveclearing (with farm distance check)
                        mode == "jg" || mode == "wc" && !KN.Root.Item("ndcwdistwc").GetValue<bool>() ||

                        // or combo mode and ignoring distance check
                        mode == "co" && !KN.Root.Item("ndcwdistco").GetValue<bool>())
                    {

                        // allow kiting between pounce if desired
                        if (mode == "jg" && target.Distance(ES.Player.ServerPosition) < 250)
                        {
                            ES.Spells["Pounce"].Cast(!KN.Root.Item("jgsticky").GetValue<bool>()
                                ? Game.CursorPos
                                : target.ServerPosition);
                        }

                        else
                            ES.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                }

                // if target is not hunted
                else
                {
                    if (target.Distance(ES.Player.ServerPosition) > ES.Spells["Pounce"].Range)
                        return;

                    var radius = ES.Player.AttackRange + ES.Player.Distance(ES.Player.BBox.Minimum) + 1;

                    // check minimum distance before pouncing
                    if (target.Distance(ES.Player.ServerPosition) > radius || 

                        // or is jungling or waveclearing (with no distance checking)
                        mode == "jg" ||  mode == "wc" && !KN.Root.Item("ndcwdistwc").GetValue<bool>() ||

                        // or combo mode with no distance checking
                        mode == "co" && !KN.Root.Item("ndcwdistco").GetValue<bool>())
                    {
                        if (target.IsChampion())
                        {
                            if (KN.Root.Item("ndcwcheck").GetValue<bool>())
                            {
                                if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 0)
                                {
                                    ES.Spells["Pounce"].CastIfHitchanceEquals(target,
                                        (HitChance)
                                            (KN.Root.Item("ndcwch").GetValue<StringList>().SelectedIndex + 3));
                                }

                                if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 1)
                                {
                                    //SPrediction.Prediction.SPredictionCast(ES.Spells["Pounce"], (Obj_AI_Hero) target,
                                    //    (HitChance)
                                    //        (KN.Root.Item("ndcwch").GetValue<StringList>().SelectedIndex + 3));
                                }
                            }

                            else
                                ES.Spells["Pounce"].Cast(target.ServerPosition);
                        }

                        // is non champion
                        if (!target.IsChampion() && mode == "wc")
                        {
                            // check pouncing near enemies
                            if (KN.Root.Item("ndcwhunt").GetValue<bool>() &&
                                target.ServerPosition.CountEnemiesInRange(450) > 0)
                                return;

                            // check pouncing under turret
                            if (KN.Root.Item("ndcwtow").GetValue<bool>() &&
                                target.ServerPosition.UnderTurret(true))
                                return;

                            // allow kiting between pounce if desired
                            if (mode == "jg" && target.Distance(ES.Player.ServerPosition) < 250)
                                ES.Spells["Pounce"].Cast(Game.CursorPos);

                            else
                                ES.Spells["Pounce"].Cast(target.ServerPosition);
                        }
                    }
                }
            }
        }


        // Cougar E Logic
        internal static void CastSwipe(Obj_AI_Base target, string mode)
        {
            if (!ES.CatForm()) 
                return;

            if (ES.SpellTimer["Swipe"].IsReady() && KN.Root.Item("ndce" + mode).GetValue<bool>())
            {
                // check valid target in range
                if (target.IsValidTarget(ES.Spells["Swipe"].Range))
                {
                    if (target.IsChampion())
                    {
                        if (KN.Root.Item("ndcecheck").GetValue<bool>())
                        {
                            if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 0)
                            {
                                ES.Spells["Swipe"].CastIfHitchanceEquals(target, (HitChance)
                                    (KN.Root.Item("ndcech").GetValue<StringList>().SelectedIndex + 3));
                            }

                            if (KN.Root.Item("prediction").GetValue<StringList>().SelectedIndex == 1)
                            {
                                //SPrediction.Prediction.SPredictionCast(ES.Spells["Swipe"], (Obj_AI_Hero) target,
                                //    (HitChance)
                                //        (KN.Root.Item("ndcwch").GetValue<StringList>().SelectedIndex + 3));
                            }
                        }
                        else
                            ES.Spells["Swipe"].Cast(target.ServerPosition);
                    }

                    else
                    {
                        // try aoe swipe if menu item > 1
                        var minhit = KN.Root.Item("ndcenum").GetValue<Slider>().Value;
                        if (minhit > 1 && mode == "wc")
                            ES.CastSmartSwipe();

                        // or cast normal
                        else
                            ES.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                }
            }
        }


        internal static void SwitchForm(Obj_AI_Base target, string mode)
        {
            // catform -> human
            if (ES.CatForm() && ES.Spells["Aspect"].IsReady() && KN.Root.Item("ndcr" + mode).GetValue<bool>())
            {
                var radius = ES.Player.AttackRange + ES.Player.Distance(ES.Player.BBox.Minimum) + 1;

                // check valid target in Q range
                if (target.IsValidTarget(ES.Spells["Javelin"].Range))
                {
                    // change form if Q is ready and meets hitchance
                    if (ES.SpellTimer["Javelin"].IsReady())
                    {
                        if (target.IsChampion())
                        {
                            if (ES.Spells["Javelin"].GetPrediction(target).Hitchance >=
                                (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3))
                            {
                                ES.Spells["Aspect"].Cast();
                            }
                        }
                        else
                        {
                            // low hitchance (collision check)
                            if (ES.Spells["Javelin"].GetPrediction(target).Hitchance >= HitChance.Low)
                                ES.Spells["Aspect"].Cast();
                        }
                    }

                    // is jungling
                    if (mode == "jg")
                    {
                        if (target.Distance(ES.Player.ServerPosition) > radius - 50)
                        {
                            if (!ES.SpellTimer["Swipe"].IsReady() && !ES.SpellTimer["Takedown"].IsReady() ||
                                Game.CursorPos.Distance(ES.Player.ServerPosition) >= 375)
                            {
                                ES.Spells["Aspect"].Cast();
                            }
                        }
                    }

                    else
                    {
                        // change to human if out of pounce range and can die
                        if (!ES.SpellTimer["Pounce"].IsReady())
                        {
                            if (target.Distance(ES.Player.ServerPosition) > radius)
                            {
                                if (ES.Player.GetAutoAttackDamage(target, true)*3 >= target.Health)
                                    ES.Spells["Aspect"].Cast();
                            }
                        }
                    }
                }
            }

            // human -> catform
            if (!ES.CatForm() && ES.Spells["Aspect"].IsReady() && KN.Root.Item("ndhr" + mode).GetValue<bool>())
            {
                if (mode == "jg" && ES.Counter < 2 && KN.Root.Item("jgaacount").GetValue<bool>())
                {
                    return;
                }

                if (mode == "gap" || mode == "wc")
                {
                    if (target.IsValidTarget(375))
                        ES.Spells["Aspect"].Cast();
                }

                // pounce only hunted
                if (KN.Root.Item("ndhrwh").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (target.IsValidTarget(ES.Spells["ExPounce"].Range) && target.IsHunted())
                        ES.Spells["Aspect"].Cast();
                }

                // pounce always any condition
                if (KN.Root.Item("ndhrwh").GetValue<StringList>().SelectedIndex == 2)
                {

                    if (target.IsValidTarget(ES.Spells["ExPounce"].Range) && target.IsHunted())
                        ES.Spells["Aspect"].Cast();

                    if (target.IsValidTarget(ES.Spells["Pounce"].Range) && !target.IsHunted())
                        ES.Spells["Aspect"].Cast();
                }
            }

            // pounce with my recommended condition
            if (KN.Root.Item("ndhrwh").GetValue<StringList>().SelectedIndex == 0)
            {
                if (!target.IsValidTarget(ES.Spells["ExPounce"].Range)) 
                    return;

                if (target.IsHunted())
                {
                    // force switch no swipe/takedown req
                    if (!KN.Root.Item("ndhrcreq").GetValue<bool>() && mode == "co" ||
                        !KN.Root.Item("ndhrjreq").GetValue<bool>() && mode == "jg")
                    {
                        ES.Spells["Aspect"].Cast();
                        return;
                    }

                    // or check if pounce timer is ready before switch
                    if (ES.SpellTimer["Pounce"].IsReady())
                    {
                        if (ES.SpellTimer["Takedown"].IsReady() || ES.SpellTimer["Swipe"].IsReady())
                            ES.Spells["Aspect"].Cast();
                    }
                }

                else
                {
                    // define our q target
                    var qtarget = TargetSelector.GetTarget(ES.Spells["Javelin"].Range,
                        TargetSelector.DamageType.Magical);
                    if (target.Distance(ES.Player.ServerPosition) <= ES.Spells["Pounce"].Range)
                    {
                        // if q target is hunted tunnel on him
                        if (qtarget.IsValidTarget(ES.Spells["Javelin"].Range) && !target.IsHunted())
                        {
                            // switch if Q is not ready in 3 or not using Q
                            if (!ES.SpellTimer["Javelin"].IsReady(3) || ES.NotLearned(ES.Spells["Javelin"]) ||
                                !KN.Root.Item("ndhq" + mode).GetValue<bool>())
                            {
                                if (target.Distance(ES.Player.ServerPosition) <= ES.Spells["Pounce"].Range)
                                    ES.Spells["Aspect"].Cast();
                            }

                            if (ES.SpellTimer["Javelin"].IsReady())
                            {
                                if (target.Distance(ES.Player.ServerPosition) <= ES.Spells["Pounce"].Range)
                                {
                                    // if we dont meet hitchance on Q target pounce nearest target
                                    var nidchance = ES.Spells["Javelin"].GetPrediction(qtarget);
                                    if (nidchance.Hitchance < (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3))
                                    {
                                        ES.Spells["Aspect"].Cast();
                                    }

                                    // or if cat damage can kill target switch
                                    if (ES.CatDamage(target) >= target.Health)
                                        ES.Spells["Aspect"].Cast();

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

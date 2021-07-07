﻿using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Apothecary
{
    // Token: 0x0200000A RID: 10
    public class AYWashUtility
    {
        // Token: 0x0600001C RID: 28 RVA: 0x00003098 File Offset: 0x00001298
        public static Thing AYWashApparel(ref Thing ingredient, Map map, bool loseQual)
        {
            var CQ = ingredient.TryGetComp<CompQuality>();
            if (CQ != null)
            {
                var quality = CQ.Quality;
                if (loseQual)
                {
                    switch (CQ.Quality)
                    {
                        case QualityCategory.Awful:
                            quality = QualityCategory.Awful;
                            break;
                        case QualityCategory.Poor:
                            quality = QualityCategory.Awful;
                            break;
                        case QualityCategory.Normal:
                            quality = QualityCategory.Poor;
                            break;
                        case QualityCategory.Good:
                            quality = QualityCategory.Normal;
                            break;
                        case QualityCategory.Excellent:
                            quality = QualityCategory.Good;
                            break;
                        case QualityCategory.Masterwork:
                            quality = QualityCategory.Excellent;
                            break;
                        case QualityCategory.Legendary:
                            quality = QualityCategory.Masterwork;
                            break;
                        default:
                            quality = QualityCategory.Normal;
                            break;
                    }
                }

                CQ.SetQuality(quality, ArtGenerationContext.Colony);
            }

            NonPublicFields.wornCorpse.SetValue(ingredient, false);
            return ingredient;
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00003124 File Offset: 0x00001324
        public static void AYWashHaulJob(Thing t)
        {
            if (!t.Spawned)
            {
                return;
            }

            var tub = t.Position.GetFirstBuilding(t.Map);
            if (tub == null || tub.def.defName != "AYApparelWashingTub")
            {
                return;
            }

            var p = tub.InteractionCell.GetFirstPawn(t.Map);
            if (p == null)
            {
                return;
            }

            var jobs = p.jobs;
            JobDef jobDef;
            if (jobs == null)
            {
                jobDef = null;
            }
            else
            {
                var curJob = jobs.curJob;
                jobDef = curJob?.def;
            }

            if (jobDef != JobDefOf.DoBill)
            {
                return;
            }

            var newHaul = HaulAIUtility.HaulToStorageJob(p, t);
            if (newHaul != null)
            {
                p.jobs.jobQueue.EnqueueFirst(newHaul, JobTag.MiscWork);
            }
        }

        // Token: 0x0200002C RID: 44
        public static class NonPublicFields
        {
            // Token: 0x04000053 RID: 83
            public static readonly FieldInfo wornCorpse = AccessTools.Field(typeof(Apparel), "wornByCorpseInt");
        }
    }
}
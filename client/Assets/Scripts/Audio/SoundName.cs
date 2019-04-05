﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SoundName {
    Theme,
    ButtonPress,
    ButtonError,
    ButtonQuit,

    trooper_Move,
    trooper_Attack,
    trooper_Death,
    trooper_Annoyed_0,
    trooper_Annoyed_1,
    trooper_Annoyed_2,
    trooper_Annoyed_3,
    trooper_Annoyed_4,
    trooper_Annoyed_5,
    trooper_Annoyed_6,
    trooper_Annoyed_7,
    trooper_Annoyed_8,
    trooper_Attack_0,
    trooper_Attack_1,
    trooper_Attack_2,
    trooper_Attack_3,
    trooper_Attack_4,
    trooper_Attack_5,
    trooper_Attack_6,
    trooper_Attack_7,
    trooper_Attack_8,
    trooper_Attack_9,
    trooper_Death_0,
    trooper_Death_1,
    trooper_Death_2,
    trooper_Death_3,
    trooper_Death_4,
    trooper_Move_0,
    trooper_Move_1,
    trooper_Move_2,
    trooper_Move_3,
    trooper_Select_0,
    trooper_Select_1,
    trooper_Select_2,
    trooper_Select_3,
    trooper_Select_4,
    trooper_Select_5,
    trooper_TakeDamage_0,

    recon_Move,
    recon_Attack,
    recon_Death,
    recon_Annoyed_0,
    recon_Annoyed_1,
    recon_Annoyed_2,
    recon_Annoyed_3,
    recon_Annoyed_4,
    recon_Annoyed_5,
    recon_Attack_0,
    recon_Attack_1,
    recon_Attack_2,
    recon_Attack_3,
    recon_Attack_4,
    recon_Attack_5,
    recon_Attack_6,
    recon_Attack_7,
    recon_Attack_8,
    recon_Attack_9,
    recon_Attack_10,
    recon_Attack_11,
    recon_Attack_12,
    recon_Attack_13,
    recon_Attack_14,
    recon_Death_0,
    recon_Death_1,
    recon_Death_2,
    recon_Death_3,
    recon_Move_0,
    recon_Move_1,
    recon_Move_2,
    recon_Move_3,
    recon_Move_4,
    recon_Move_5,
    recon_Move_6,
    recon_Move_7,
    recon_Move_8,
    recon_Select_0,
    recon_Select_1,
    recon_Select_2,
    recon_TakeDamage_0,
    recon_TakeDamage_1,
    recon_TakeDamage_2,

    steamer_Move,
    steamer_Attack,
    steamer_Death,
    steamer_Annoyed_0,
    steamer_Annoyed_1,
    steamer_Annoyed_2,
    steamer_Annoyed_3,
    steamer_Annoyed_4,
    steamer_Annoyed_5,
    steamer_Annoyed_6,
    steamer_Annoyed_7,
    steamer_Annoyed_8,
    steamer_Attack_0,
    steamer_Attack_1,
    steamer_Attack_2,
    steamer_Attack_3,
    steamer_Attack_4,
    steamer_Attack_5,
    steamer_Attack_6,
    steamer_Attack_7,
    steamer_Attack_8,
    steamer_Attack_9,
    steamer_Death_0,
    steamer_Death_1,
    steamer_Death_2,
    steamer_Death_3,
    steamer_Death_4,
    steamer_Death_5,
    steamer_Move_0,
    steamer_Move_1,
    steamer_Move_2,
    steamer_Move_3,
    steamer_Select_0,
    steamer_Select_1,
    steamer_Select_2,
    steamer_Select_3,
    steamer_Select_4,
    steamer_TakeDamage_0,

    pewpew_Move,
    pewpew_Attack,
    pewpew_Death,
    pewpew_Annoyed_0,
    pewpew_Annoyed_1,
    pewpew_Annoyed_2,
    pewpew_Annoyed_3,
    pewpew_Attack_0,
    pewpew_Attack_1,
    pewpew_Attack_2,
    pewpew_Attack_3,
    pewpew_Attack_4,
    pewpew_Attack_5,
    pewpew_Attack_6,
    pewpew_Attack_7,
    pewpew_Death_0,
    pewpew_Death_1,
    pewpew_Death_2,
    pewpew_Death_3,
    pewpew_Move_0,
    pewpew_Move_1,
    pewpew_Move_2,
    pewpew_Move_3,
    pewpew_Move_4,
    pewpew_Move_5,
    pewpew_Move_6,
    pewpew_Select_0,
    pewpew_Select_1,
    pewpew_Select_2,
    pewpew_Select_3,
    pewpew_TakeDamage_0,

    compensator_Move,
    compensator_Attack,
    compensator_Death,
    compensator_Annoyed_0,
    compensator_Annoyed_1,
    compensator_Annoyed_2,
    compensator_Annoyed_3,
    compensator_Annoyed_4,
    compensator_Annoyed_5,
    compensator_Annoyed_6,
    compensator_Annoyed_7,
    compensator_Annoyed_8,
    compensator_Annoyed_9,
    compensator_Attack_0,
    compensator_Attack_1,
    compensator_Attack_2,
    compensator_Attack_3,
    compensator_Attack_4,
    compensator_Attack_5,
    compensator_Attack_6,
    compensator_Attack_7,
    compensator_Attack_8,
    compensator_Attack_9,
    compensator_Attack_10,
    compensator_Attack_11,
    compensator_Attack_12,
    compensator_Attack_13,
    compensator_Attack_14,
    compensator_Attack_15,
    compensator_Attack_16,
    compensator_Death_0,
    compensator_Death_1,
    compensator_Death_2,
    compensator_Move_0,
    compensator_Move_1,
    compensator_Select_0,
    compensator_Select_1,
    compensator_Select_2,
    compensator_Select_3,
    compensator_Select_4,
    compensator_TakeDamage_0,
    compensator_TakeDamage_1,
    compensator_TakeDamage_2,
    compensator_TakeDamage_3,

    piercing_tungsten_Move,
    piercing_tungsten_Attack,
    piercing_tungsten_Death,
    piercing_tungsten_Ability_0,
    piercing_tungsten_Ability_1,
    piercing_tungsten_Ability_2,
    piercing_tungsten_Annoyed_0,
    piercing_tungsten_Annoyed_1,
    piercing_tungsten_Annoyed_2,
    piercing_tungsten_Annoyed_3,
    piercing_tungsten_Annoyed_4,
    piercing_tungsten_Annoyed_5,
    piercing_tungsten_Annoyed_6,
    piercing_tungsten_Annoyed_7,
    piercing_tungsten_Attack_0,
    piercing_tungsten_Move_0,
    piercing_tungsten_Move_1,
    piercing_tungsten_Move_2,
    piercing_tungsten_Move_3,
    piercing_tungsten_Move_4,
    piercing_tungsten_Select_0,
    piercing_tungsten_Select_1,
    piercing_tungsten_Select_2,
    piercing_tungsten_Select_3,
    piercing_tungsten_Select_4,
    piercing_tungsten_TakeDamage_0,
    piercing_tungsten_TakeDamage_1,

    support_sandman_Move,
    support_sandman_Attack,
    support_sandman_Death,
    support_sandman_Ability_0,
    support_sandman_Ability_1,
    support_sandman_Ability_2,
    support_sandman_Annoyed_0,
    support_sandman_Annoyed_1,
    support_sandman_Annoyed_2,
    support_sandman_Annoyed_3,
    support_sandman_Annoyed_4,
    support_sandman_Attack_0,
    support_sandman_Attack_1,
    support_sandman_Attack_2,
    support_sandman_Attack_3,
    support_sandman_Attack_4,
    support_sandman_Death_0,
    support_sandman_Death_1,
    support_sandman_Move_0,
    support_sandman_Move_1,
    support_sandman_Move_2,
    support_sandman_Move_3,
    support_sandman_Move_4,
    support_sandman_Move_5,
    support_sandman_Move_6,
    support_sandman_Select_0,
    support_sandman_Select_1,
    support_sandman_Select_2,
    support_sandman_Select_3,
    support_sandman_Select_4,
    support_sandman_Select_5,
    support_sandman_Select_6,
    support_sandman_TakeDamage_0,
    support_sandman_TakeDamage_1,
    support_sandman_TakeDamage_2,

    foundation_Move,
    foundation_Attack,
    foundation_Death,
    foundation_Annoyed_0,
    foundation_Annoyed_1,
    foundation_Annoyed_2,
    foundation_Annoyed_3,
    foundation_Attack_0,
    foundation_Attack_1,
    foundation_Attack_2,
    foundation_Attack_3,
    foundation_Death_0,
    foundation_Death_1,
    foundation_Move_0,
    foundation_Move_1,
    foundation_Move_2,
    foundation_Move_3,
    foundation_Move_4,
    foundation_Move_5,
    foundation_Select_0,
    foundation_Select_1,
    foundation_TakeDamage_0,
    foundation_TakeDamage_1,
    foundation_TakeDamage_2,

    powerSurge_Move,
    powerSurge_Attack,
    powerSurge_Death,
    powerSurge_Annoyed_0,
    powerSurge_Annoyed_1,
    powerSurge_Annoyed_2,
    powerSurge_Annoyed_3,
    powerSurge_Attack_0,
    powerSurge_Attack_1,
    powerSurge_Attack_2,
    powerSurge_Attack_3,
    powerSurge_Attack_4,
    powerSurge_Attack_5,
    powerSurge_Attack_6,
    powerSurge_Attack_7,
    powerSurge_Attack_8,
    powerSurge_Attack_9,
    powerSurge_Death_0,
    powerSurge_Death_1,
    powerSurge_Death_2,
    powerSurge_Death_3,
    powerSurge_Death_4,
    powerSurge_Move_0,
    powerSurge_Move_1,
    powerSurge_Move_2,
    powerSurge_Move_3,
    powerSurge_Move_4,
    powerSurge_Move_5,
    powerSurge_Move_6,
    powerSurge_Select_0,
    powerSurge_Select_1,
    powerSurge_Select_2,
    powerSurge_Select_3,
    powerSurge_Select_4,
    powerSurge_Select_5,
    powerSurge_Select_6,
    powerSurge_TakeDamage_0,
    powerSurge_TakeDamage_1,

    midas_Move,
    midas_Attack,
    midas_Death,
    midas_Annoyed_0,
    midas_Annoyed_1,
    midas_Annoyed_2,
    midas_Annoyed_3,
    midas_Annoyed_4,
    midas_Annoyed_5,
    midas_Annoyed_6,
    midas_Annoyed_7,
    midas_Annoyed_8,
    midas_Annoyed_9,
    midas_Annoyed_10,
    midas_Annoyed_11,
    midas_Attack_0,
    midas_Attack_1,
    midas_Attack_2,
    midas_Attack_3,
    midas_Attack_4,
    midas_Attack_5,
    midas_Attack_6,
    midas_Attack_7,
    midas_Attack_8,
    midas_Attack_9,
    midas_Attack_10,
    midas_Attack_11,
    midas_Attack_12,
    midas_Death_0,
    midas_Death_1,
    midas_Death_2,
    midas_Move_0,
    midas_Move_1,
    midas_Move_2,
    midas_Move_3,
    midas_Move_4,
    midas_Move_5,
    midas_Move_6,
    midas_Select_0,
    midas_Select_1,
    midas_Select_2,
    midas_Select_3,
    midas_Select_4,
    midas_TakeDamage_0,
    midas_TakeDamage_1,
    midas_TakeDamage_2,

    claymore_Move,
    claymore_Attack,
    claymore_Death,
    claymore_Annoyed_0,
    claymore_Annoyed_1,
    claymore_Annoyed_2,
    claymore_Attack_0,
    claymore_Attack_1,
    claymore_Attack_2,
    claymore_Attack_3,
    claymore_Attack_4,
    claymore_Attack_5,
    claymore_Attack_6,
    claymore_Attack_7,
    claymore_Attack_8,
    claymore_Attack_9,
    claymore_Death_0,
    claymore_Death_1,
    claymore_Death_2,
    claymore_Death_3,
    claymore_Death_4,
    claymore_Death_5,
    claymore_Move_0,
    claymore_Move_1,
    claymore_Move_2,
    claymore_Move_3,
    claymore_Move_4,
    claymore_Move_5,
    claymore_Move_6,
    claymore_Move_7,
    claymore_Move_8,
    claymore_Select_0,
    claymore_Select_1,
    claymore_Select_2,
    claymore_Select_3,
    claymore_Select_4,
    claymore_Select_5,
    claymore_TakeDamage_0,
    claymore_TakeDamage_1,

    heavy_albarn_Move,
    heavy_albarn_Attack,
    heavy_albarn_Death,
    heavy_albarn_Ability_0,
    heavy_albarn_Ability_1,
    heavy_albarn_Annoyed_0,
    heavy_albarn_Annoyed_1,
    heavy_albarn_Annoyed_2,
    heavy_albarn_Annoyed_3,
    heavy_albarn_Annoyed_4,
    heavy_albarn_Annoyed_5,
    heavy_albarn_Annoyed_6,
    heavy_albarn_Attack_0,
    heavy_albarn_Attack_1,
    heavy_albarn_Attack_2,
    heavy_albarn_Attack_3,
    heavy_albarn_Death_0,
    heavy_albarn_Death_1,
    heavy_albarn_Death_2,
    heavy_albarn_Death_3,
    heavy_albarn_Death_4,
    heavy_albarn_Move_0,
    heavy_albarn_Select_0,
    heavy_albarn_Select_1,
    heavy_albarn_Select_2,
    heavy_albarn_Select_3,
    heavy_albarn_Select_4,
    heavy_albarn_Select_5,
    heavy_albarn_TakeDamage_0,
    heavy_albarn_TakeDamage_1,


    light_adren_Move,
    light_adren_Attack,
    light_adren_Death,
    light_adren_Ability_0,
    light_adren_Ability_1,
    light_adren_Ability_2,
    light_adren_Ability_3,
    light_adren_Ability_4,
    light_adren_Ability_5,
    light_adren_Annoyed_0,
    light_adren_Annoyed_1,
    light_adren_Annoyed_2,
    light_adren_Annoyed_3,
    light_adren_Annoyed_4,
    light_adren_Annoyed_5,
    light_adren_Attack_0,
    light_adren_Attack_1,
    light_adren_Attack_2,
    light_adren_Attack_3,
    light_adren_Death_0,
    light_adren_Death_1,
    light_adren_Death_2,
    light_adren_Death_3,
    light_adren_Move_0,
    light_adren_Move_1,
    light_adren_Move_2,
    light_adren_Move_3,
    light_adren_Move_4,
    light_adren_Select_0,
    light_adren_Select_1,
    light_adren_Select_2,
    light_adren_Select_3,
    light_adren_TakeDamage_0,
}

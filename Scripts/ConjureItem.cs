using UnityEngine;
using System;

using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;

namespace ConjureItem
{
    public class ConjureItem : BaseEntityEffect
    {
        public static readonly string EffectKey = "ConjureItem";

        //variants
        struct VariantProperties
        {
            public string subGroupKey;
            public EffectProperties effectProperties;
            public WeaponMaterialTypes varMatType;
        }

        static readonly VariantProperties[] variants = new VariantProperties[]
        {
            new VariantProperties()
            {
                subGroupKey = "Iron",
                varMatType = WeaponMaterialTypes.Iron,
            },
            new VariantProperties()
            {
                subGroupKey = "Steel",
                varMatType = WeaponMaterialTypes.Steel,
            },
            new VariantProperties()
            {
                subGroupKey = "Silver",
                varMatType = WeaponMaterialTypes.Silver,
            },
            new VariantProperties()
            {
                subGroupKey = "Elven",
                varMatType = WeaponMaterialTypes.Elven,
            },
            new VariantProperties()
            {
                subGroupKey = "Dwarven",
                varMatType = WeaponMaterialTypes.Dwarven,
            },
            new VariantProperties()
            {
                subGroupKey = "Mithril",
                varMatType = WeaponMaterialTypes.Mithril,
            },
            new VariantProperties()
            {
                subGroupKey = "Adamantium",
                varMatType = WeaponMaterialTypes.Adamantium,
            },
            new VariantProperties()
            {
                subGroupKey = "Ebony",
                varMatType = WeaponMaterialTypes.Ebony,
            },
            new VariantProperties()
            {
                subGroupKey = "Orcish",
                varMatType = WeaponMaterialTypes.Orcish,
            },
            new VariantProperties()
            {
                subGroupKey = "Daedric",
                varMatType = WeaponMaterialTypes.Daedric,
            }
        };

        // override to return correct properties
        public override EffectProperties Properties
        {
            get { return variants[currentVariant].effectProperties; }
        }

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.SupportDuration = true;
            properties.ShowSpellIcon = false;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.DurationCosts = MakeEffectCosts(60, 120);

            variantCount = variants.Length;

            // unique variant properties
            for (int i = 0; i < variantCount; i++)
            {
                variants[i].effectProperties = properties; // default property shortcut
                variants[i].effectProperties.Key = string.Format("{0}-{1}", EffectKey, variants[i].subGroupKey);

                // variant prices

                // top tier 
                if (variants[i].subGroupKey == "Daedric")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(20, 20, 3000);
                }
                else if (variants[i].subGroupKey == "Ebony")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(20, 20, 3000);
                }
                else if (variants[i].subGroupKey == "Orcish")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(20, 20, 3000);
                }

                // mithril tier
                else if (variants[i].subGroupKey == "Mithril")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(15, 15, 1500);
                }
                else if (variants[i].subGroupKey == "Adamantium")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(15, 15, 1500);
                }

                // mer tier
                else if (variants[i].subGroupKey == "Elven")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(8, 8, 750);
                }
                else if (variants[i].subGroupKey == "Dwarven")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(8, 8, 750);
                }

                // man tier
                else if (variants[i].subGroupKey == "Steel")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(4, 4, 300);
                }
                else if (variants[i].subGroupKey == "Silver")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(4, 4, 300);
                }

                // base tier
                else if (variants[i].subGroupKey == "Iron")
                {
                    variants[i].effectProperties.DurationCosts = MakeEffectCosts(2, 2, 100);
                }
            }

        }

        public override string GroupName => "Conjure Item";
        public override string SubGroupName => variants[currentVariant].subGroupKey;

        const string effectDescription = "Conjures an item bound to the realm by the casters Magicka";
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookDescription();

        private TextFile.Token[] GetSpellMakerDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                GroupName,
                effectDescription,
                "Duration: How Long Conjured Items Last",
                "Chance: N/A");
        }

        private TextFile.Token[] GetSpellBookDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                GroupName,
                "Duration: How Long Conjured Items Lasts",
                "Chance: N/A",
                effectDescription);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            PromptPlayer();
        }

        void PromptPlayer()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Target must be player - no effect on other entities
            if (entityBehaviour != DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntityBehaviour)
                return;

            // menu creator
            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PopWindow();
            DaggerfallListPickerWindow itemPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
            itemPicker.OnItemPicked += itemPicked;
            itemPicker.ListBox.AddItem("Weapons");
            itemPicker.ListBox.AddItem("Armour");
            itemPicker.ListBox.AddItem("Clothing");
            itemPicker.ListBox.AddItem("Items");

            if (itemPicker.ListBox.Count > 0)
                DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(itemPicker);
        }

        // sub menus
        protected virtual void itemPicked(int index, string itemString)
        {
            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PopWindow();

            switch (itemString)
            {
                case "Weapons":
                    DaggerfallListPickerWindow wepPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                    wepPicker.OnItemPicked += wepPicked;

                    wepPicker.ListBox.AddItem("Short Blades");
                    wepPicker.ListBox.AddItem("Long Blades");
                    wepPicker.ListBox.AddItem("Axes");
                    wepPicker.ListBox.AddItem("Blunt Weapons");
                    wepPicker.ListBox.AddItem("Bows");

                    if (wepPicker.ListBox.Count > 0)
                        DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(wepPicker);
                    break;
                case "Armour":
                    DaggerfallListPickerWindow armPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                    armPicker.OnItemPicked += armPicked;

                    armPicker.ListBox.AddItem("Armour");
                    armPicker.ListBox.AddItem("Buckler");
                    armPicker.ListBox.AddItem("Kite Shield");
                    armPicker.ListBox.AddItem("Round Shield");
                    armPicker.ListBox.AddItem("Tower Shield");

                    if (armPicker.ListBox.Count > 0)
                        DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(armPicker);
                    break;
                case "Clothing":
                    DaggerfallListPickerWindow cloPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                    cloPicker.OnItemPicked += cloPicked;

                    cloPicker.ListBox.AddItem("Coming Soon");

                    if (cloPicker.ListBox.Count > 0)
                        DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(cloPicker);
                    break;
                case "Items":
                    DaggerfallListPickerWindow itmPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                    itmPicker.OnItemPicked += itmPicked;
                    itmPicker.ListBox.AddItem("Coming Soon");

                    if (itmPicker.ListBox.Count > 0)
                        DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(itmPicker);
                    break;
            }
        }
            //weapons
            protected virtual void wepPicked(int id, string wepString)
            {
                DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PopWindow();

                switch (wepString)
                {
                    case "Short Blades":
                        DaggerfallListPickerWindow swrdPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                        swrdPicker.OnItemPicked += chosePicked;

                        swrdPicker.ListBox.AddItem("Dagger");
                        swrdPicker.ListBox.AddItem("Tanto");
                        swrdPicker.ListBox.AddItem("Shortsword");
                        swrdPicker.ListBox.AddItem("Wakazashi");

                        if (swrdPicker.ListBox.Count > 0)
                            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(swrdPicker);
                        break;
                    case "Long Blades":
                        DaggerfallListPickerWindow lswrdPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                        lswrdPicker.OnItemPicked += chosePicked;

                        lswrdPicker.ListBox.AddItem("Broadsword");
                        lswrdPicker.ListBox.AddItem("Saber");
                        lswrdPicker.ListBox.AddItem("Longsword");
                        lswrdPicker.ListBox.AddItem("Katana");
                        lswrdPicker.ListBox.AddItem("Claymore");
                        lswrdPicker.ListBox.AddItem("DaiKatana");

                        if (lswrdPicker.ListBox.Count > 0)
                            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(lswrdPicker);
                        break;
                    case "Axes":
                        DaggerfallListPickerWindow axePicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                        axePicker.OnItemPicked += chosePicked;

                        axePicker.ListBox.AddItem("Battleaxe");
                        axePicker.ListBox.AddItem("Waraxe");


                        if (axePicker.ListBox.Count > 0)
                            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(axePicker);
                        break;
                    case "Blunt Weapons":
                        DaggerfallListPickerWindow bluntPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                        bluntPicker.OnItemPicked += chosePicked;

                        bluntPicker.ListBox.AddItem("Broadsword");
                        bluntPicker.ListBox.AddItem("Saber");
                        bluntPicker.ListBox.AddItem("Longsword");
                        bluntPicker.ListBox.AddItem("Katana");
                        bluntPicker.ListBox.AddItem("Claymore");
                        bluntPicker.ListBox.AddItem("DaiKatana");

                        if (bluntPicker.ListBox.Count > 0)
                            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(bluntPicker);
                        break;
                    case "Bows":
                        DaggerfallListPickerWindow bowPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                        bowPicker.OnItemPicked += chosePicked;

                        bowPicker.ListBox.AddItem("Shortbow");
                        bowPicker.ListBox.AddItem("Longbow");

                        if (bowPicker.ListBox.Count > 0)
                            DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(bowPicker);
                        break;
                }
            }

            //armours
            protected virtual void armPicked(int id, string armString)
            {
                DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PopWindow();

                DaggerfallListPickerWindow armPicker = new DaggerfallListPickerWindow(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, DaggerfallWorkshop.Game.DaggerfallUI.UIManager.TopWindow);
                armPicker.OnItemPicked += chosePicked;

                switch (armString)
                {
                case "Armour":
                    armPicker.ListBox.AddItem("Leather");
                    armPicker.ListBox.AddItem("Chain");
                    armPicker.ListBox.AddItem("Plate");
                    break;
                case "Buckler":
                    armPicker.ListBox.AddItem("Leather Buckler");
                    armPicker.ListBox.AddItem("Chain Buckler");
                    armPicker.ListBox.AddItem("Metal Buckler");
                    break;
                case "Kite Shield":
                    armPicker.ListBox.AddItem("Leather Kite Shield");
                    armPicker.ListBox.AddItem("Chain Kite Shield");
                    armPicker.ListBox.AddItem("Metal Kite Shield");
                    break;
                case "Round Shield":
                    armPicker.ListBox.AddItem("Leather Round Shield");
                    armPicker.ListBox.AddItem("Chain Round Shield");
                    armPicker.ListBox.AddItem("Metal Round Shield");
                    break;
                case "Tower Shield":
                    armPicker.ListBox.AddItem("Leather Tower Shield");
                    armPicker.ListBox.AddItem("Chain Tower Shield");
                    armPicker.ListBox.AddItem("Metal Tower Shield");
                    break;
            }

                if (armPicker.ListBox.Count > 0)
                    DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PushWindow(armPicker);


            }

            //clothings
            protected virtual void cloPicked(int id, string cloString)
            {

            }
            
            //items
            protected virtual void itmPicked(int id, string itmString)
            {

            }

            // item creation
            void chosePicked(int id, string itemType)
            {
                DaggerfallWorkshop.Game.DaggerfallUI.UIManager.PopWindow();

                DaggerfallUnityItem item = null;
                // this will be decided based on spell variant
                WeaponMaterialTypes matType = variants[currentVariant].varMatType;

                item = CreateItem(itemType, matType);

                if (item != null)
                    DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                else if (item == null) { } // do nothing
            }
            
            // create methods
            DaggerfallUnityItem CreateItem(string itemType, WeaponMaterialTypes matType)
            {
            
                Genders gender = DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Gender;
                Races race = DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Race;
                DaggerfallUnityItem item = null;

                // weapon to armour converter
                ArmorMaterialTypes armMatType = ArmorMaterialTypes.Iron;
                switch (matType)
                {
                    case WeaponMaterialTypes.Iron:
                        armMatType = ArmorMaterialTypes.Iron;
                        break;
                    case WeaponMaterialTypes.Steel:
                        armMatType = ArmorMaterialTypes.Steel;
                        break;
                    case WeaponMaterialTypes.Silver:
                        armMatType = ArmorMaterialTypes.Silver;
                        break;
                    case WeaponMaterialTypes.Elven:
                        armMatType = ArmorMaterialTypes.Elven;
                        break;
                    case WeaponMaterialTypes.Dwarven:
                        armMatType = ArmorMaterialTypes.Dwarven;
                        break;
                    case WeaponMaterialTypes.Mithril:
                        armMatType = ArmorMaterialTypes.Mithril;
                        break;
                    case WeaponMaterialTypes.Adamantium:
                        armMatType = ArmorMaterialTypes.Adamantium;
                        break;
                    case WeaponMaterialTypes.Ebony:
                        armMatType = ArmorMaterialTypes.Ebony;
                        break;
                    case WeaponMaterialTypes.Orcish:
                        armMatType = ArmorMaterialTypes.Orcish;
                        break;
                    case WeaponMaterialTypes.Daedric:
                        armMatType = ArmorMaterialTypes.Daedric;
                        break;
                }

                switch (itemType)
                {
                    //Weapons
                    //Short Swords
                    case "Dagger":
                        item = ItemBuilder.CreateWeapon(Weapons.Dagger, matType);
                        break;
                    case "Tanto":
                        item = ItemBuilder.CreateWeapon(Weapons.Tanto, matType);
                        break;
                    case "Shortsword":
                        item = ItemBuilder.CreateWeapon(Weapons.Shortsword, matType);
                        break;
                    case "Wakazashi":
                        item = ItemBuilder.CreateWeapon(Weapons.Wakazashi, matType);
                        break;
                    // Long Swords
                    case "Broadsword":
                        item = ItemBuilder.CreateWeapon(Weapons.Broadsword, matType);
                        break;
                    case "Saber":
                        item = ItemBuilder.CreateWeapon(Weapons.Saber, matType);
                        break;
                    case "Longsword":
                        item = ItemBuilder.CreateWeapon(Weapons.Longsword, matType);
                        break;
                    case "Katana":
                        item = ItemBuilder.CreateWeapon(Weapons.Katana, matType);
                        break;
                    case "Claymore":
                        item = ItemBuilder.CreateWeapon(Weapons.Claymore, matType);
                        break;
                    case "DaiKatana":
                        item = ItemBuilder.CreateWeapon(Weapons.Dai_Katana, matType);
                        break;
                    //Axes
                    case "BattleAxe":
                        item = ItemBuilder.CreateWeapon(Weapons.Battle_Axe, matType);
                        break;
                    case "WarAxe":
                        item = ItemBuilder.CreateWeapon(Weapons.War_Axe, matType);
                        break;
                    //Blunt Weapons
                    case "Staff":
                        item = ItemBuilder.CreateWeapon(Weapons.Staff, matType);
                        break;
                    case "Mace":
                        item = ItemBuilder.CreateWeapon(Weapons.Mace, matType);
                        break;
                    case "Flail":
                        item = ItemBuilder.CreateWeapon(Weapons.Flail, matType);
                        break;
                    case "Warhammer":
                        item = ItemBuilder.CreateWeapon(Weapons.Warhammer, matType);
                        break;
                    //Missile Weapons
                    case "Shortbow":
                        item = ItemBuilder.CreateWeapon(Weapons.Short_Bow, matType);
                        break;
                    case "Longbow":
                        item = ItemBuilder.CreateWeapon(Weapons.Long_Bow, matType);
                        break;
                //Shield
                case "Leather Buckler":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Buckler, ArmorMaterialTypes.Leather);
                    break;
                case "Chain Buckler":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Buckler, ArmorMaterialTypes.Chain);
                    break;
                case "Metal Buckler":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Buckler, armMatType);
                    break;
                case "Leather Kite Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Kite_Shield, ArmorMaterialTypes.Leather);
                    break;
                case "Chain Kite Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Kite_Shield, ArmorMaterialTypes.Chain);
                    break;
                case "Metal Kite Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Kite_Shield, armMatType);
                    break;
                case "Leather Round Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Round_Shield, ArmorMaterialTypes.Leather);
                    break;
                case "Chain Round Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Round_Shield, ArmorMaterialTypes.Chain);
                    break;
                case "Metal Round Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Round_Shield, armMatType);
                    break;
                case "Leather Tower Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Tower_Shield, ArmorMaterialTypes.Leather);
                    break;
                case "Chain Tower Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Tower_Shield, ArmorMaterialTypes.Chain);
                    break;
                case "Metal Tower Shield":
                    item = ItemBuilder.CreateArmor(gender, race, Armor.Tower_Shield, armMatType);
                    break;
                //Armour
                case "Leather":
                        createArmour(gender, race, ArmorMaterialTypes.Leather);
                        break;
                    case "Chain":
                        createArmour(gender, race, ArmorMaterialTypes.Chain);
                        break;
                    case "Plate":
                        createArmour(gender, race, armMatType);
                        break;

                    //Clothing
                    //Items
            }

                // Set lifetime of item based on spell duration
                uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
                if (item == null) { }
                else
                {
                    item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                }
                return item;
            }


            // this will create the whole armour set
            void createArmour(Genders gender, Races race, ArmorMaterialTypes matType)
            {
                DaggerfallUnityItem item = null;
                uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

                item = ItemBuilder.CreateArmor(gender, race, Armor.Helm, matType);
                if (item != null)
                    DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                item = null;
                item = ItemBuilder.CreateArmor(gender, race, Armor.Cuirass, matType);
                if (item != null)
                    DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                item = null;
                item = ItemBuilder.CreateArmor(gender, race, Armor.Left_Pauldron, matType);
                if (item != null)
                    DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                item = null;
                item = ItemBuilder.CreateArmor(gender, race, Armor.Right_Pauldron, matType);
                if (item != null)
                    DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                item = null;
                item = ItemBuilder.CreateArmor(gender, race, Armor.Gauntlets, matType);
                if (item != null)
                    DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                item = null;
                item = ItemBuilder.CreateArmor(gender, race, Armor.Greaves, matType);
                if (item != null)
                        DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
                item = null;
                item = ItemBuilder.CreateArmor(gender, race, Armor.Boots, matType);
                if (item != null)
                        DaggerfallWorkshop.Game.GameManager.Instance.PlayerEntity.Items.AddItem(item);
                item.TimeForItemToDisappear = (uint)(gameMinutes + RoundsRemaining);
            }


    }
}

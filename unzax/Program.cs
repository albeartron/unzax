using System;
using System.IO;

namespace unzax
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = null;

            // check if enough command line arguments
            if (args.Length != 1)
            {
                Console.WriteLine("Error: Invalid command line argument(s).");
                return;
            }

            // set file name from first command line argument
            fileName = (string)args.GetValue(0);
            fileName = System.IO.Path.GetFullPath(fileName);

            // check if file exists
            if (!System.IO.File.Exists(fileName))
            {
                Console.WriteLine("Error: File does not exist.");
                return;
            }

            // open the file
            BinaryReader inputFile = new BinaryReader(new FileStream(fileName, FileMode.Open));

            Int32 xUnits, yUnits;
            Int32 segmentLength;
            Int32 textureIdx;
            float locationX, locationY;
            float rotation;
            float scalingX, scalingY;
            string textureName;
            string strFlag;
            bool has_strFlag;
            Int32 intFlag;
            bool has_intFlag;

            xUnits = inputFile.ReadInt32();
            yUnits = inputFile.ReadInt32();
            Console.WriteLine("xUnits\t" + xUnits);
            Console.WriteLine("yUnits\t" + yUnits);

            Console.Write("index\t");
            Console.Write("segmentLength\t");
            Console.Write("segmentIndex\t");
            Console.Write("textureIdx\t");
            Console.Write("locationX\t");
            Console.Write("locationY\t");
            Console.Write("rotation\t");
            Console.Write("scalingX\t");
            Console.Write("scalingY\t");
            Console.Write("textureName\t");
            Console.Write("type\t");
            Console.Write("strFlag\t");
            Console.Write("intFlag\n");


            // read non-entity segments
            for (int i = 0; i < 19; i++)
            {
                segmentLength = inputFile.ReadInt32();
                for (int j = 0; j < segmentLength; j++)
                {
                    /* segment structure:
                    // int32 length...
                    //  int32   idx
                    //  float   locationX
                    //  float   locationY
                    //  float   rotation
                    //  float   scalingX
                    //  float   scalingY
                    //  string  textureName
                    //  ..get texture type and do:
                    //      3:  string strFlag, int32 intFlag
                    //      !2: return
                    //      ..get texture idx and do:
                    //          9, 10, 11, 13, 14, 15, 22, 29, 30:
                    //          string strFlag
                    //          int32 intFlag
                    //              if intFlag is 22, set it to -1
                    */
                    textureIdx = inputFile.ReadInt32();
                    locationX = inputFile.ReadSingle();
                    locationY = inputFile.ReadSingle();
                    rotation = inputFile.ReadSingle();
                    scalingX = inputFile.ReadSingle();
                    scalingY = inputFile.ReadSingle();
                    textureName = inputFile.ReadString();

                    has_strFlag = false;
                    has_intFlag = false;
                    strFlag = null;
                    intFlag = -1;

                    // creature/monstor type textures?
                    if (getTextureType(textureName) == 3)
                    {
                        strFlag = inputFile.ReadString();
                        intFlag = inputFile.ReadInt32();
                        has_strFlag = true;
                        has_intFlag = true;
                    }
                    // same as !=2 -> return?
                    
                    // map textures?
                    if (getTextureType(textureName) == 2)
                    {
                        switch (kludgeFlags(textureName, textureIdx))
                        {
                            case 9:
                            case 10:
                            case 11:
                            case 13:
                            case 14:
                            case 15:
                            case 22:
                            case 29:
                            case 30:
                                string str = inputFile.ReadString();
                                strFlag = !(str == "") ? str : (string)null;
                                // replace new line with comma
                                try
                                {
                                    strFlag = strFlag.Replace("\r\n", ",");
                                }
                                catch (Exception)
                                { }
                                intFlag = inputFile.ReadInt32();
                                //if (originalCell.flags != 22)
                                //    break;
                                //this.intFlag = -1;
                                if (kludgeFlags(textureName, textureIdx) == 22)
                                {
                                    intFlag = -1;
                                }
                                break;
                            default:
                                break;
                        }
                        
                        has_strFlag = true;
                        has_intFlag = true;
                    }
                    
                    

                    Console.Write(i + "\t");
                    Console.Write(segmentLength + "\t");
                    Console.Write(j + "\t");
                    Console.Write(textureIdx + "\t");
                    Console.Write(locationX + "\t");
                    Console.Write(locationY + "\t");
                    Console.Write(rotation + "\t");
                    Console.Write(scalingX + "\t");
                    Console.Write(scalingY + "\t");
                    Console.Write(textureName + "\t");
                    Console.Write(getTextureType(textureName) + "\t");
                    if (has_strFlag)
                    {
                        Console.Write(strFlag);
                    }
                    Console.Write("\t");
                    if (has_intFlag)
                    {
                        Console.Write(intFlag);
                    }
                    Console.Write("\n");
                }
            }

            // read entity segment (19)
            segmentLength = inputFile.ReadInt32();
            for (int j = 0; j < segmentLength; j++)
            {
                // segment structure:
                // int32 length...
                //  int32   idx
                //  float   locationX
                //  float   locationY
                //  int32 intFlag
                //  string textureName
                //  string  strFlag

                textureIdx = inputFile.ReadInt32();
                locationX = inputFile.ReadSingle();
                locationY = inputFile.ReadSingle();
                intFlag = inputFile.ReadInt32();
                textureName = inputFile.ReadString();
                strFlag = inputFile.ReadString();
                try
                {
                    strFlag = strFlag.Replace("\r\n", ",");
                }
                catch (Exception)
                { }

                Console.Write(19 + "\t");
                Console.Write(segmentLength + "\t");
                Console.Write(j + "\t");
                Console.Write(textureIdx + "\t");
                Console.Write(locationX + "\t");
                Console.Write(locationY + "\t");
                Console.Write("\t");
                Console.Write("\t");
                Console.Write("\t");
                Console.Write(textureName + "\t");
                Console.Write("\t");
                Console.Write(strFlag + "\t");
                Console.Write(intFlag + "\n");
            }

            // read next part
            for (int index1 = 0; index1 < xUnits; ++index1)    //from 0 to 1587
            {
                int index2 = 0;
            label_8:
                int num1 = inputFile.ReadInt32();
                if (num1 != -1)
                {
                    int num2 = (int)inputFile.ReadByte();
                    for (int index3 = 0; index3 < num1; ++index3)
                    {
                        //this.col[index1, index2].col = num2;
                        //Console.Write("[\t" + index1 + "\t" + index2 + "]\t" + num2 + "\n");
                        if (num2 != 0)
                        {
                            //Console.Write("[\t" + index1 + "\t" + index2 + "]\t" + num2 + "\n");
                        }
                        ++index2;
                    }
                    goto label_8;
                }
            }
            // and the next part
            for (int index1 = 0; index1 < xUnits; ++index1)
            {
                int index2 = 0;
            label_16:
                int num1 = inputFile.ReadInt32();
                if (num1 != -1)
                {
                    int num2 = (int)inputFile.ReadByte();
                    for (int index3 = 0; index3 < num1; ++index3)
                    {
                        //this.col[index1, index2].layer = num2;
                        if (num2 !=0)
                        {
                            //Console.Write("[\t" + index1 + "\t" + index2 + "]\t" + num2 + "\n");
                        }
                        //Console.Write("[\t" + index1 + "\t" + index2 + "]\t" + num2 + "\n");
                        ++index2;
                    }
                    goto label_16;
                }
            }

            // then read sequence
            //int32 length
            // from 0 to length,
            //  read <sequence>
            //   string name
            //   int32 length
            //   ..set up an array of <SequenceState> that's <length> long
            //   ..at each <SequenceState>, set up a <SequenceLayer> that's 20 long
            //   ..at each <SequenceLayer>,
            //      readboolean (if true..)
            //      read a <SequenceLayer>
            //          int32 length
            //          <SequenceSeg>[length]
            //              int32 ID
            //              float rotation
            //              float locationX
            //              float locationY
            //              float scalingX
            //              float scalingY
            //     int32 colX
            //     int32 colY
            //     int32 col (x)
            //     int32 col (y)
            //     read col(x)*col(y) int32's
            //     int32 lines
            //     string script *lines times
            //      

            string name = inputFile.ReadString();
            int length = inputFile.ReadInt32();
            //this.states = new Sequence.SequenceState[length];
            //for (int index = 0; index < length; ++index)
            //    this.states[index] = new Sequence.SequenceState();
            for (int index1 = 0; index1 < length; ++index1)
            {
                //this.states[index1].layer = new SequenceLayer[20];
                //for (int index2 = 0; index2 < this.states[index1].layer.Length; ++index2)
                for (int index2 = 0; index2 < 20; ++index2)
                {
                    if (inputFile.ReadBoolean())
                    {
                        //this.states[index1].layer[index2] = new SequenceLayer();
                        //this.states[index1].layer[index2].Read(reader);
                        int length2 = inputFile.ReadInt32();
                        //this.seg = new SequenceSeg[length2];
                        for (int index = 0; index < length2; ++index)
                        { 
                            //this.seg[index] = new SequenceSeg(reader);
                            int ID = inputFile.ReadInt32();
                            float rotation2 = inputFile.ReadSingle();
                            float locX = inputFile.ReadSingle();
                            float locY = inputFile.ReadSingle();
                            float scalingX2 = inputFile.ReadSingle();
                            float scalingY2 = inputFile.ReadSingle();
                        }
                    }
                }
                int colX = inputFile.ReadInt32();
                int colY = inputFile.ReadInt32();
                //this.states[index1].col = new int[reader.ReadInt32(), reader.ReadInt32()];
                //2d array of ints, colX2 x colY2
                int colX2 = inputFile.ReadInt32();
                int colY2 = inputFile.ReadInt32();
                //for (int index2 = 0; index2 < this.states[index1].col.GetUpperBound(0) + 1; ++index2)
                for (int index2 = 0; index2 < colX2 + 1; ++index2)
                {
                    //for (int index3 = 0; index3 < this.states[index1].col.GetUpperBound(1) + 1; ++index3)
                    for (int index3 = 0; index3 < colY2 + 1; ++index3)
                    {
                        //this.states[index1].col[index2, index3] = reader.ReadInt32();
                        int value = inputFile.ReadInt32();
                    }
                }
            }
            int lines = inputFile.ReadInt32();
            //this.script = new SequenceScript(lines, this);
            for (int i = 0; i < lines; ++i)
            {
                //this.script.AddLine(i, reader.ReadString());
                string scriptLine = inputFile.ReadString();
            }
            //Vector2 vector2 = new Vector2();
            /* no file reads here...
            int num = 0;
            for (int index1 = 0; index1 < length; ++index1)
            {
                for (int index2 = 0; index2 < this.states[index1].layer.Length; ++index2)
                {
                    if (this.states[index1].layer[index2] != null)
                    {
                        for (int index3 = 0; index3 < this.states[index1].layer[index2].seg.Length; ++index3)
                        {
                            vector2 += this.states[index1].layer[index2].seg[index3].loc;
                            ++num;
                        }
                    }
                }
            }
            */
            //if (num > 0)
            //    vector2 /= (float)num;
            //this.center = vector2;
            //this.script.ReadScript();


            // close the file
            inputFile.Close();
        }

        static int getTextureType(string inString)
        {
            switch (inString)
            {
                case "axe_anchor":
                case "axe_blue":
                case "axe_broken":
                case "axe_bronze":
                case "axe_bull":
                case "axe_cleaver":
                case "axe_crescent":
                case "axe_crypt":
                case "axe_double":
                case "axe_giant":
                case "axe_gold":
                case "axe_great":
                case "axe_iron":
                case "axe_ruins":
                case "axe_tribe":
                case "bow_gold":
                case "bow_hawk":
                case "bow_long":
                case "bow_red":
                case "bow_stonesoul":
                case "bow_wood":
                case "consumables":
                case "crossbow":
                case "crossbow_blades":
                case "crossbow_dragon":
                case "crossbow_mummy":
                case "crossbow_red":
                case "dagger":
                case "dagger_curved":
                case "dagger_jagged":
                case "dagger_poison":
                case "dagger_shiv":
                case "dagger_white":
                case "endingtrees":
                case "gun_flintlock":
                case "gun_heavy":
                case "gun_ruins":
                case "gun_teeth":
                case "gun_white":
                case "halberd":
                case "hammer_crypt":
                case "hammer_golem":
                case "hammer_great":
                case "hammer_ogre":
                case "hammer_scepter":
                case "logos":
                case "logosegs":
                case "mace":
                case "mace_blacksmith":
                case "mace_cruel":
                case "mace_flanged":
                case "mace_hammer":
                case "mace_pot":
                case "mace_puffer":
                case "mace_squid":
                case "mace_tribe":
                case "naginata":
                case "polearm_ref":
                case "poleaxe":
                case "pole_judge":
                case "pole_pendulum":
                case "pole_ranseur":
                case "scythe":
                case "scythe_bones":
                case "scythe_moon":
                case "scythe_pendulum":
                case "scythe_spoon":
                case "scythe_wicked":
                case "shield_antler":
                case "shield_banner":
                case "shield_black":
                case "shield_blue":
                case "shield_branches":
                case "shield_bronze":
                case "shield_buckler":
                case "shield_bug":
                case "shield_cleric":
                case "shield_dread":
                case "shield_eagle":
                case "shield_golem":
                case "shield_heater":
                case "shield_iron":
                case "shield_ornate":
                case "shield_oval":
                case "shield_paladin":
                case "shield_rotten":
                case "shield_saltknight":
                case "shield_silver":
                case "shield_skulls":
                case "shield_snakes":
                case "shield_tower":
                case "shield_tree":
                case "shield_viking":
                case "shield_whispers":
                case "shield_wood":
                case "shield_wood_tower":
                case "smashables":
                case "smashbits":
                case "spear":
                case "spear_circle":
                case "spear_cloak":
                case "spear_imp":
                case "spear_oar":
                case "spear_pilum":
                case "spear_pitchfork":
                case "spear_vines":
                case "spear_wicked":
                case "sprites":
                case "staff_bones":
                case "staff_conduit":
                case "staff_ivory":
                case "staff_lakewitch":
                case "staff_wood":
                case "swooshes":
                case "sword_bronze":
                case "sword_claymore":
                case "sword_cleaver":
                case "sword_cloak":
                case "sword_crab":
                case "sword_cutlass":
                case "sword_dark_claymore":
                case "sword_deadking":
                case "sword_dread":
                case "sword_flame":
                case "sword_flamewhip":
                case "sword_gun":
                case "sword_iron":
                case "sword_katana":
                case "sword_messer":
                case "sword_nameless":
                case "sword_onikatana":
                case "sword_pirate":
                case "sword_runes":
                case "sword_rust":
                case "sword_scissor":
                case "sword_short":
                case "sword_sparksor":
                case "sword_whip":
                case "torch":
                case "trident":
                case "trident_blue":
                case "wand_beak":
                case "wand_bones":
                case "wand_butterfly":
                case "wand_stag":
                case "wand_wood":
                case "war_scythe":
                case "weapons":
                case "whip":
                case "whip_bones":
                case "whip_chain":
                case "whip_flame":
                case "whip_monster":
                case "whip_poison":
                case "whip_wraith":
                    return 0;
                case "armor_alchemist":
                case "armor_alchemist2":
                case "armor_black":
                case "armor_blacksmith":
                case "armor_blacksmith2":
                case "armor_blades":
                case "armor_bluering":
                case "armor_bluering2":
                case "armor_boat":
                case "armor_boat2":
                case "armor_bronze":
                case "armor_captain":
                case "armor_captain2":
                case "armor_chain":
                case "armor_chain2":
                case "armor_chef":
                case "armor_chef2":
                case "armor_cleric":
                case "armor_cleric2":
                case "armor_cloak":
                case "armor_crypt":
                case "armor_cutqueen":
                case "armor_cutqueen2":
                case "armor_dapper":
                case "armor_dapper2":
                case "armor_devil":
                case "armor_dish":
                case "armor_dish2":
                case "armor_dread":
                case "armor_fat":
                case "armor_gold":
                case "armor_gold_archer":
                case "armor_gold_archer2":
                case "armor_golem":
                case "armor_guide":
                case "armor_hawk":
                case "armor_hunter":
                case "armor_hunter2":
                case "armor_jester":
                case "armor_jester2":
                case "armor_knight":
                case "armor_lakewitch":
                case "armor_lakewitch2":
                case "armor_leather":
                case "armor_mage":
                case "armor_mage2":
                case "armor_merchant":
                case "armor_merchant2":
                case "armor_monk":
                case "armor_monk2":
                case "armor_monsterwitch":
                case "armor_monsterwitch2":
                case "armor_nameless":
                case "armor_ninja":
                case "armor_ninja2":
                case "armor_paladin":
                case "armor_patched":
                case "armor_patched2":
                case "armor_pirate":
                case "armor_pirate2":
                case "armor_purpledress":
                case "armor_purpledress2":
                case "armor_rags":
                case "armor_rags2":
                case "armor_reddress":
                case "armor_reddress2":
                case "armor_redmage":
                case "armor_redmage2":
                case "armor_ribs":
                case "armor_rotten":
                case "armor_rotten2":
                case "armor_salad":
                case "armor_salad2":
                case "armor_saltknight":
                case "armor_samurai":
                case "armor_shroud":
                case "armor_slug":
                case "armor_slug2":
                case "armor_smallclothes":
                case "armor_sorceror":
                case "armor_sorceror2":
                case "armor_split":
                case "armor_stag":
                case "armor_tater":
                case "armor_thief":
                case "armor_thief2":
                case "armor_tribe":
                case "armor_tribe2":
                case "armor_tunic":
                case "armor_tunic2":
                case "armor_white":
                case "armor_whitedress":
                case "armor_whitedress2":
                case "armor_wolf":
                case "beard_beard":
                case "beard_chops":
                case "beard_epic":
                case "beard_epic_trim":
                case "beard_fancy":
                case "beard_fancy_trim":
                case "beard_goatee":
                case "beard_hipster":
                case "beard_moustache":
                case "beard_soul":
                case "beard_truegoat":
                case "boots_alchemist":
                case "boots_black":
                case "boots_blacksmith":
                case "boots_blades":
                case "boots_bluering":
                case "boots_boat":
                case "boots_bronze":
                case "boots_captain":
                case "boots_chain":
                case "boots_chef":
                case "boots_chef2":
                case "boots_cleric":
                case "boots_cloak":
                case "boots_crypt":
                case "boots_cutqueen":
                case "boots_dapper":
                case "boots_devil":
                case "boots_dish":
                case "boots_dread":
                case "boots_fat":
                case "boots_gold_archer":
                case "boots_golem":
                case "boots_guide":
                case "boots_hawk":
                case "boots_heavy_gold":
                case "boots_hunter":
                case "boots_jester":
                case "boots_knight":
                case "boots_lakewitch":
                case "boots_leather":
                case "boots_mage":
                case "boots_merchant":
                case "boots_monk":
                case "boots_monsterwitch":
                case "boots_nameless":
                case "boots_ninja":
                case "boots_paladin":
                case "boots_patched":
                case "boots_pirate":
                case "boots_purpledress":
                case "boots_rags":
                case "boots_reddress":
                case "boots_redmage":
                case "boots_ribs":
                case "boots_rotten":
                case "boots_salad":
                case "boots_saltknight":
                case "boots_samurai":
                case "boots_shroud":
                case "boots_slug":
                case "boots_smallclothes":
                case "boots_sorceror":
                case "boots_split":
                case "boots_stag":
                case "boots_tater":
                case "boots_thief":
                case "boots_thief2":
                case "boots_tribe":
                case "boots_tunic":
                case "boots_white":
                case "boots_whitedress":
                case "boots_wolf":
                case "eyebrows":
                case "eyes":
                case "eyes2":
                case "eyes2d":
                case "eyes2f":
                case "eyes2g":
                case "eyes2l":
                case "eyes2o":
                case "eyes2s":
                case "eyes2v":
                case "eyesd":
                case "eyesf":
                case "eyesg":
                case "eyesl":
                case "eyeso":
                case "eyess":
                case "eyesv":
                case "gloves_black":
                case "gloves_blacksmith":
                case "gloves_blades":
                case "gloves_bluering":
                case "gloves_bronze":
                case "gloves_chain":
                case "gloves_cloak":
                case "gloves_crypt":
                case "gloves_cutqueen":
                case "gloves_dapper":
                case "gloves_dish":
                case "gloves_dread":
                case "gloves_fat":
                case "gloves_gold_archer":
                case "gloves_golem":
                case "gloves_guide":
                case "gloves_heavy_gold":
                case "gloves_hunter":
                case "gloves_knight":
                case "gloves_leather":
                case "gloves_mage":
                case "gloves_merchant":
                case "gloves_monk":
                case "gloves_nameless":
                case "gloves_ninja":
                case "gloves_paladin":
                case "gloves_patched":
                case "gloves_rags":
                case "gloves_redmage":
                case "gloves_ribs":
                case "gloves_rotten":
                case "gloves_saltknight":
                case "gloves_samurai":
                case "gloves_shroud":
                case "gloves_slug":
                case "gloves_split":
                case "gloves_stag":
                case "gloves_tater":
                case "gloves_thief":
                case "gloves_white":
                case "hair_balding":
                case "hair_bob":
                case "hair_buns":
                case "hair_buzz":
                case "hair_crew":
                case "hair_cropped":
                case "hair_dreads":
                case "hair_dual":
                case "hair_dumbbob":
                case "hair_emo":
                case "hair_firestarter":
                case "hair_hat_bob":
                case "hair_hat_buns":
                case "hair_hat_dreads":
                case "hair_hat_dual":
                case "hair_hat_emo":
                case "hair_hat_hip":
                case "hair_hat_messy":
                case "hair_hat_natural":
                case "hair_hat_princess":
                case "hair_hat_shag":
                case "hair_hat_smoothdreads":
                case "hair_hat_spiky":
                case "hair_hat_wavy":
                case "hair_hip":
                case "hair_messy":
                case "hair_mohawk":
                case "hair_natural":
                case "hair_princess":
                case "hair_shag":
                case "hair_short":
                case "hair_shortfro":
                case "hair_shortround":
                case "hair_smoothdreads":
                case "hair_spiky":
                case "hair_topknot":
                case "hair_wavy":
                case "helm_alchemist":
                case "helm_black":
                case "helm_blades":
                case "helm_bloodeye":
                case "helm_bluering":
                case "helm_boat":
                case "helm_bronze":
                case "helm_chain":
                case "helm_chef":
                case "helm_claymask":
                case "helm_cleric":
                case "helm_cloak":
                case "helm_crypt":
                case "helm_cutqueen":
                case "helm_dapper":
                case "helm_devil":
                case "helm_dish":
                case "helm_dread":
                case "helm_fat":
                case "helm_gold":
                case "helm_gold_archer":
                case "helm_golem":
                case "helm_guide":
                case "helm_hawk":
                case "helm_heavy_gold":
                case "helm_hunter":
                case "helm_jester":
                case "helm_knight":
                case "helm_knight_closed":
                case "helm_lakewitch":
                case "helm_leather":
                case "helm_merchant":
                case "helm_monsterwitch":
                case "helm_nameless":
                case "helm_ninja":
                case "helm_paladin":
                case "helm_patched":
                case "helm_pigeonmask":
                case "helm_pirate":
                case "helm_pumpkin":
                case "helm_purpledress":
                case "helm_rags":
                case "helm_reddress":
                case "helm_redmage":
                case "helm_ribs":
                case "helm_rotten":
                case "helm_saltknight":
                case "helm_samurai":
                case "helm_shroud":
                case "helm_slug":
                case "helm_split":
                case "helm_tater":
                case "helm_thief":
                case "helm_torturer":
                case "helm_torture_bow":
                case "helm_tribe":
                case "helm_white":
                case "helm_whitedress":
                    return 1;
                case "castle":
                case "cave":
                case "dome":
                case "dungeon":
                case "entities":
                case "forest":
                case "glows":
                case "lab":
                case "ocean":
                case "ruins":
                case "ship":
                case "swamp":
                case "tomb":
                case "village":
                    return 2;
                case "alchemist":
                case "angler":
                case "archer":
                case "arms":
                case "bandages":
                case "bandit":
                case "bat":
                case "birdfetus":
                case "birdy":
                case "blob":
                case "bluetroll":
                case "bluewraith":
                case "blue_blob":
                case "broken":
                case "bull":
                case "butterfly":
                case "cat":
                case "chest":
                case "clay":
                case "cloak":
                case "crab":
                case "crow":
                case "crusher":
                case "cryptguard":
                case "cult":
                case "cutqueen":
                case "deadjudge":
                case "deadking":
                case "deadknight":
                case "deadlord":
                case "dog":
                case "doll":
                case "dragon":
                case "dread":
                case "drowned":
                case "eyeball":
                case "eyescorpion":
                case "fallen":
                case "fiend":
                case "flamejet":
                case "frog":
                case "gaoler":
                case "gasbag":
                case "ghost":
                case "ghostwitch":
                case "gold_archer":
                case "golem":
                case "greenspider":
                case "guardian":
                case "guardian_cleric":
                case "guardian_gen":
                case "guardian_rotten":
                case "guardmage":
                case "guardmage_cleric":
                case "hangman":
                case "hawk":
                case "hippogriff":
                case "horsehead":
                case "horseman":
                case "imp":
                case "inquisitor":
                case "jester":
                case "kingcat":
                case "knight_axe":
                case "knight_bronze":
                case "knight_white":
                case "lakewitch":
                case "lamprey":
                case "leviathan":
                case "lich":
                case "littlecorn":
                case "marauder":
                case "maraudzom":
                case "mimic":
                case "monster":
                case "monsterwitch":
                case "mummy":
                case "nameless":
                case "nightmare":
                case "obelisk":
                case "ogre":
                case "phantasm":
                case "piranha":
                case "pirate":
                case "poisonrock":
                case "potato":
                case "rags":
                case "raider":
                case "ruinaxe":
                case "sack":
                case "sailor":
                case "saltbat":
                case "saltknight":
                case "scarecrow":
                case "skeleton":
                case "skeleton_archer":
                case "skeleton_tribe":
                case "skin":
                case "skin2":
                case "skin2d":
                case "skin2f":
                case "skin2g":
                case "skin2l":
                case "skin2o":
                case "skin2s":
                case "skin2v":
                case "skind":
                case "skinf":
                case "sking":
                case "skinl":
                case "skino":
                case "skins":
                case "skinsalad":
                case "skinv":
                case "skull":
                case "soldier":
                case "sorceror":
                case "spider":
                case "spider_blue":
                case "split":
                case "squiddragon":
                case "squidface":
                case "squidmimic":
                case "stonesoul":
                case "switch":
                case "taplion":
                case "tapunicorn":
                case "tortured":
                case "torturer":
                case "torturer_bow":
                case "torturetree":
                case "traps":
                case "troll":
                case "unicorn":
                case "whiterabbit":
                case "witch":
                case "wraith":
                case "zombie":
                case "zomblack":
                case "zombow":
                    return 4;
                default:
                    return -1;
            }
        }

        static int kludgeFlags(string inString, int idxIn)
        {
            switch (inString)
            {
                case "entities":
                    switch (idxIn)
                    {
                        case 1:
                            return 11;
                        case 3:
                            return 14;
                        case 4:
                            return 13;
                        case 5:
                            return 15;
                        case 9:
                            return 29;
                        case 10:
                            return 30;
                        default:
                            return -1;
                    }
                case "dungeon":
                    switch (idxIn)
                    {
                        case 33:
                            return 9;
                        default:
                            return -1;
                    }
                case "castle":
                    switch (idxIn)
                    {
                        case 103:
                            return 9;
                        case 107:
                            return 22;
                        default:
                            return -1;
                    }
                default:
                    return -1;
            }
        }
    }
}

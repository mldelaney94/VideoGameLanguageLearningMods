from polib import POEntry, POFile
import polib
import pinyiniser as pyer

# Formatting fixes: spaced-out punctuation/markup back to correct form
PUNCTUATION_REPLACEMENTS = dict[str, str](sorted({
  " 。 ": "。", " 。": "。", "。 ": "。",
  "， ": "，",
  " ，": "，",
  " ， ": "，",
  "： ": "：",
  " ：": "：",
  " ： ": "：",
  " : ": ":",
  " \\ n": "\\n", "\\ n": "\\n",
  " ！ ": "！", " ！": "！", "！ ": "！",
  " !": "!",
  " \\ ": "\\",
  " \"": "\"", "\" ": "\"",
  " ？": "？",
  "？ ": "？",
  " ？ ": "？",
  "> ": ">", " , ": ",",
  " — — ": "——", " -- > ": "-->",
  "* * * ": "***",
  "* ": "*", " *":"*", " * ": "*",
  "\u201c ": "\u201c", " \u201d": "\u201d",
  " \u201c": "\u201c", "\u201d ": "\u201d ",
  "… …": "……", "…… ": "……", " ……": "……",
  "（ ": "（", " ）": "）",
  "] ": "]", " [": "[",
  " ]": "]", "[ ": "[",
  ". . .": "...",
  " _ ": "_",
  "  -  ": "-",
  "- ": "-",
  "+ ": "+",
  " % ": "%",
  # Strip leading space after newlines in pinyin (caused by ' '.join on \n segments)
  "\n ": "\n",
  # General multi-space collapsing — the pinyiniser produces 3–5+ space gaps
  # between English words (Lone Star, Tir Tairngire, Stuffer Shack, etc.),
  # around game tags ({{/GM}}, {{/CC}}), and after colons in data entries.
  # Longest first so .replace() fully collapses any run in one pass.
  "         ": " ",
  "        ": " ",
  "       ": " ",
  "      ": " ",
  "     ": " ",
  "    ": " ",
  "   ": " ",
  " / ": "/",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

# Template variable fixes: spaced-out game variables back to correct form
TEMPLATE_TOKENS = {
  # $(l.*) / $(s.*)
  "$(l.name)",
  "$(l.Name)",
  "$(l.sir)",
  "$(l.Sir)",
  "$(l.man)",
  "$(l.guy)",
  "$(l.race)",
  "$(l.Race)",
  "$(l.he)",
  "$(l.him)",
  "$(l.his)",
  "$(l.hisher)",
  "$(l.honorific)",
  "$(l.freund)",
  "$(s.name)",
  "$(s.Name)",
  "$(s.man)",
  "$(s.guy)",
  "$(s.he)",
  "$(s.him)",
  "$(s.hisher)",
  "$(s.race)",
  # $+(l.*) / $+(s.*)
  "$+(l.name)",
  "$+(l.guy)",
  "$+(l.sir)",
  "$+(l.he)",
  "$+(l.hisher)",
  "$+(s.he)",
  "$+(s.hisher)",
  # $++(l.*) / $++(L.*)
  "$++(l.name)",
  "$++(l.race)",
  "$++(l.man)",
  "$++(L.NAME)",
  "$++(L.RACE)",
  "$++(L.HIM)",
  # $(global.*)
  "$(global.cheats_used)",
  "$(global.dmg_rcv)",
  "$(global.dmg_snd)",
  "$(global.elapsed_time)",
  "$(global.good_combat_time)",
  "$(global.good_friendly_time)",
  "$(global.heal_item)",
  "$(global.kills)",
  "$(global.mission_complete)",
  # $(player.*)
  "$(player.deaths)",
  "$(player.dmg_snd)",
  "$(player.kills)",
  # $(team.*)
  "$(team.cheats_used)",
  "$(team.good_combat_time)",
  "$(team.good_friendly_time)",
  "$(team.kills)",
  # $(scene.*)
  "$(scene.ActiveGasArea)",
  "$(scene.apocalyptic)",
  "$(scene.badfantasy)",
  "$(scene.CafeSpecial)",
  "$(scene.countBombsRemaining)",
  "$(scene.countPoisonClock)",
  "$(scene.CountRoundsBomb)",
  "$(scene.cyberpunk)",
  "$(scene.fantasy)",
  "$(scene.himher)",
  "$(scene.iBountyToCollect)",
  "$(scene.iNumCCCFound)",
  "$(scene.iNumMagnifikerToSell)",
  "$(scene.numUnreadMessages)",
  "$(scene.numWaterPumpsActive)",
  "$(scene.postapocalyptic)",
  "$(scene.RoundsLeft)",
  "$(scene.sciencefiction)",
  "$(scene.sFakeName)",
  "$(scene.space)",
  "$(scene.steampunk)",
  "$(scene.str_MeOrUs)",
  "$(scene.str_RedOrGreen)",
  "$(scene.superhero)",
  "$(scene.western)",
  # $(story.*)
  "$(story.a3_Endgame_s4_LoadingScreen)",
  "$(story.date_Aztechnology)",
  "$(story.date_Chemie)",
  "$(story.date_Humanis)",
  "$(story.date_Sewer)",
  "$(story.date_Trust_Blitz)",
  "$(story.date_Trust_Eiger)",
  "$(story.Global_AliceFunds)",
  "$(story.Global_HavenLoadingScreen)",
  "$(story.Global_Skillcheck_Easy)",
  "$(story.Global_Skillcheck_Hard)",
  "$(story.Global_Skillcheck_Medium)",
  "$(story.Haven_Global_SamuelDonations)",
  "$(story.Haven_Global_SamuelGoal)",
  "$(story.Hub_countTranspondersPlaced)",
}

# Markup tag fixes: spaced-out game markup tags back to correct form
MARKUP_TOKENS = {
  "{{/GM}}",
  "{{GM}}",
  "{{CC}}",
  "{{/CC}}",
}

# Proper name pinyin fixes: spaced pinyin syllables joined into proper names
# NOTE: names the pinyiniser already squishes (Pa4ke1, Ji2nuo4, De2li4la1, etc.)
# are omitted — only SPACED forms that actually appear in output are listed.
PROPER_NAME_REPLACEMENTS = dict(sorted({
  # Dragonfall Extended / Berlin
  # Vauclair — 102 hits
  "wo4 ke4lai2er3": "Wo4ke4lai2er3",
  # Adrian Vauclair — 51 hits
  "e1 de2 li3 an1": "E1de2li3an1",
  # Glory — 100+ hits
  "ge2 luo4 li2": "Ge2luo4li2",
  # Dietrich — 100+ hits
  "di2 te4 li3 xi1": "Di2te4li3xi1",
  # Eiger — 100+ hits
  "ai4 ge2 er3": "Ai4ge2er3",
  # Monika — 103 hits
  "mo4 ni1 ka3": "Mo4ni1ka3",
  # Monika's shadow — 39 hits
  "mo4 ni1 ka3 de5 ying3xiang4": "Mo4ni1ka3de5ying3xiang4",
  # Marta — 100+ hits
  "ma3 er3 ta3": "Ma3er3ta3",
  # Audran — 100+ hits
  "ao4 de2 lan2": "Ao4de2lan2",
  # Blitz — 100+ hits
  "bu4 li3 ci2": "Bu4li3ci2",
  # Harrow — 39 hits
  "xie1 hai2": "Xie1hai2",
  # Herr Schmidt — 46 hits
  "te4 xian1": "Te4xian1",
  # Goldschmidt — 12 hits
  "ge1 de2 shi1": "Ge1de2shi1",
  # Zaak — 53 hits
  "zha1 ke4": "Zha1ke4",
  # Green Winters — 102 hits
  "wen1 te4 si1": "Wen1te4si1",
  # Ezkibel — 52 hits
  "ai1 zi1 ji1 bei4": "Ai1ziji1bei4",
  # Yuli — 47 hits
  "you2 li3": "You2li3",
  # Beckenbauer — 20 hits
  "bei4 ken3 bao4 er3": "Bei4ken3bao4er3",
  # Volker Stahl — 50 hits (surname)
  "si1 ta3 er3": "Si1ta3er3",
  # Volker Stahl — 15 hits (given name)
  "wo4 er3 ke4": "Wo4er3ke4",
  # Lucky Strike — 7 hits
  "xing4 yun4 xing1": "Xing4yun4xing1",
  # Kreuzbasar — 100+ hits
  "ke4 luo4 yi1 ci2 ji2": "Ke4luo4yi1ci2ji2",
  # Kreuzbasar (short) — 101 hits
  "ke4 luo4 yi1": "Ke4luo4yi1",
  # Feuerschwinge — 100+ hits
  "huo3 yi4": "Huo3yi4",
  # Lofwyr — 37 hits
  "luo4 fu1 wei2 er3": "Luo4fu1wei2er3",
  # Nebelherr — 3 hits (partial squish)
  "nei4 bai3 er3 he4er3": "Nei4bai3er3he4er3",
  # Kaltenstein — 2 hits (partial squish)
  "ka3er3 teng2 shi1 tan3": "Ka3er3teng2shi1tan3",
  # Harz — 7 hits
  "ha1 er3 ci2": "Ha1er3ci2",
  # Knight Errant — 96 hits
  "xia2 qi2": "Xia2qi2",
  # Aztechnology — 102 hits
  "e1 zi1 te4 ke1": "E1zi1te4ke1",
  # Schockwellenreiter — 40 hits
  "bo1 qi2": "Bo1qi2",
  # Falkenrath — 2 hits
  "fa3 ken3 la1 si1": "Fa3ken3la1si1",
  # Saeder-Krupp — 42 hits
  "sai4 de2": "Sai4de2",
  # Rabengeister — 31 hits
  "ya1 ling2": "Ya1ling2",
  # Shadowrunner — 93 hits
  "ying3 kuang2": "Ying3kuang2",
  # Shadowrunner — 101 hits (alt transliteration)
  "ben1 zhe3": "Ben1zhe3",
  # APEX — 15 hits
  "yu4 xi4": "Yu4xi4",
  # Diehl Defense — 9 hits
  "di2 er3 fang2wu4": "Di2er3fang2wu4",
  # Amsel — 100 hits
  "e1 mu3 ze2 er3": "E1mu3ze2er3",
  # Hasenkamp — 79 hits
  "ha1 sen1 kan3 pu3": "Ha1sen1kan3pu3",
  # Plotz — 46 hits
  "pu3 luo4 ci2": "Pu3luo4ci2",
  # Harfeld Manor — 43 hits
  "ha1 fei1 er3 de2 zhuang1": "Ha1fei1er3de2zhuang1",
  # Harfeld — 46 hits
  "ha1 fei1 er3 de2": "Ha1fei1er3de2",
  # Aljernon (alt transliteration) — 43 hits
  "e1 er3 jie2 nong2": "E1er3jie2nong2",
  # Jana — 42 hits
  "jian3 na4": "Jian3na4",
  # SOX — 41 hits
  "sa4 luo4 lu2": "Sa4luo4lu2",
  # Silke — 31 hits
  "xi1 er3 ke4": "Xi1er3ke4",
  # Enstad — 31 hits
  "shi1 ta3 de2": "Shi1ta3de2",
  # Herr Fuchs — 27 hits
  "fu4 ke4 si1": "Fu4ke4si1",
  # Heimer — 21 hits
  "hai3 mo4": "Hai3mo4",
  # Kami — 20 hits
  "ka3 mi3": "Ka3mi3",
  # Ruby — 19 hits
  "lou4 bi3": "Lou4bi3",
  # Titonius Rex — 17 hits
  "tai4 tuo1 ni2 wu1 si1": "Tai4tuo1ni2wu1si1",
  # Das Kesselhaus — 15 hits
  "guo1 lu2 fang2": "Guo1lu2fang2",
  # Xolotl — 8 hits
  "xiu1 luo4 te4 er3": "Xiu1luo4te4er3",
  # Rabengeister (alt) — 6 hits
  "ya1 shen2": "Ya1shen2",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

MISC_REPLACEMENTS = dict(sorted({
  # Email / URLs — 1 hit each
  "support @ hbs -studios . com": "support@hbs-studios.com",
  "harebrained -schemes . com": "harebrained-schemes.com",
  "twitter / webeharebrained": "twitter/webeharebrained",
  "fb / harebrainedschemes": "fb/harebrainedschemes",
  # Act names — 4 hits
  "( Not counted for enemy Scaling )": "(Not counted for enemy Scaling)",
  # Weapon / model designators
  "Sai4de2 -ke4lu3bo2": "Sai4de2-ke4lu3bo2",  # 42 hits
  "SM -3": "SM-3",  # 4 hits
  "AK -97": "AK-97",  # 2 hits
  "MA -2100": "MA-2100",  # 3 hits
  "MGL -6": "MGL-6",  # 2 hits
  "CTY -360": "CTY-360",  # 2 hits
  "APEX057 -": "APEX057-",  # 1 hit
  # Game stat modifiers — 11 hits
  "jing1 zhun3 du4 -5": "jing1 zhun3 du4-5",
  "jing1 zhun3 du4 -1": "jing1 zhun3 du4-1",
  # UI labels with spaced slash — 12 hits
  "shi1fa3 / zhou4 shu4": "shi1fa3/zhou4shu4",
  "shi4 / fou3": "shi4/fou3",
  # Version strings — 8 hits
  "v . 1 . 11": "v.1.11",
  "v . 1 . 01": "v.1.01",
  "v . 2 . 71": "v.2.71",
  "v . 1 . 1": "v.1.1",
  # Matrix/VR dates YYYY -MM -DD — 26 hits total
  " -01 -": "-01-",
  " -02 -": "-02-",
  " -03 -": "-03-",
  " -04 -": "-04-",
  " -05 -": "-05-",
  " -06 -": "-06-",
  " -07 -": "-07-",
  " -08 -": "-08-",
  " -09 -": "-09-",
  " -10 -": "-10-",
  " -11 -": "-11-",
  " -12 -": "-12-",
  # Numbered list steps
  "1 . ": "1. ",
  "2 . ": "2. ",
  "3 . ": "3. ",
  # Act name double-space after colon — 14 hits
  ":  ": ": ",
  # File extensions
  " . com": ".com",
  " . exe": ".exe",
  # Newsnet reference: / / xin1wen2wang3 / / → //xin1wen2wang3//
  "/ /": "//",
  "( 0 )": "(0)",
  "{ 0 }": "{0}",
  " 、 ": "、",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

def main():
  d = pyer.get_dictionary()

  SPECIAL_TOKENS = MARKUP_TOKENS | TEMPLATE_TOKENS | pyer.special_tokens
  REPLACEMENTS = PROPER_NAME_REPLACEMENTS | PUNCTUATION_REPLACEMENTS | MISC_REPLACEMENTS

  berlin_po = get_po_from_mo('./materials/translations/berlin_original.mo')
  dragonfall_extended_po = get_po_from_mo('./materials/translations/Dragonfall Extended_original.mo')

  berlin = add_pinyin(berlin_po, d, SPECIAL_TOKENS, REPLACEMENTS)
  dragonfall_extended = add_pinyin(dragonfall_extended_po, d, SPECIAL_TOKENS, REPLACEMENTS)
  berlin.save()
  dragonfall_extended.save()

  berlin.save_as_mofile('berlin.mo')
  dragonfall_extended.save_as_mofile('Dragonfall Extended.mo')

def add_pinyin(pofile: POFile, d, SPECIAL_TOKENS, REPLACEMENTS: dict[str, str]):
  for idx, entry in enumerate[POEntry](pofile):
    chinese, pinyin = pyer.get_segments_and_pinyin(entry.msgstr, d, SPECIAL_TOKENS)
    chinese = '\u200B'.join(chinese)
    pinyin = ' '.join(pinyin)
    if (chinese.count('\n') > 1):
      pofile[idx].msgstr = chinese + '\n\n' + pinyin
    else:
      pofile[idx].msgstr = chinese + '\n' + pinyin
    for old, new in REPLACEMENTS.items():
      pofile[idx].msgstr = pofile[idx].msgstr.replace(old, new)
  return pofile

def get_po_from_mo(path_to_mo):
  mo = polib.mofile(path_to_mo)
  # shouldn't have to save these files but the library only deals with files
  mo.save_as_pofile(path_to_mo.replace('.mo', '.po'))

  return polib.pofile(path_to_mo.replace('.mo', '.po'))


if __name__ == "__main__":
  main()

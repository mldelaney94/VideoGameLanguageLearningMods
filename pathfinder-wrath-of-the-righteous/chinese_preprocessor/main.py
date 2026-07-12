import json
import re
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
  "         ": " ",
  "        ": " ",
  "       ": " ",
  "      ": " ",
  "     ": " ",
  "    ": " ",
  "   ": " ",
  " / ": "/",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

# Proper name pinyin fixes: spaced pinyin syllables joined into proper names
PROPER_NAME_REPLACEMENTS = dict[str, str](sorted({
  # Sarkoris
  "sa4 kuo4 li4": "Sa4kuo4li4",
  "sa4 kuo4 li4 ren2": "Sa4kuo4li4ren2",
  # Kenabres
  "kan3 na4 bu4 li4": "Kan3na4bu4li4",
  # Iomedae
  "ai4 ao4 mei2 dai4": "Ai4ao4mei2dai4",
  # Desna
  "de2 si1 na4": "De2si1na4",
  # Ustalav
  "wu1 si1 ta3 la1fu1": "Wu1si1ta3la1fu1",
  "wu1 si1 ta3 la1 fu1ren5": "Wu1si1ta3la1fu1ren5",
  # Dahak
  "da2 ha1 ke4": "Da2ha1ke4",
  # Mendev
  "meng2 di4 wei2": "Meng2di4wei2",
  "meng2 di4 wei2 ren2": "Meng2di4wei2ren2",
  # Apsu
  "e1 bo2 su1": "E1bo2su1",
  # Galfrey
  "gao1 fu2 rui4": "Gao1fu2rui4",
  "meng2 di4 wei2 de5 gao1 fu2 rui4": "Meng2di4wei2de5gao1fu2rui4",
  # Axis
  "zhou2xin1 cheng2": "Zhou2xin1cheng2",
  "zhou2xin1 yu4": "Zhou2xin1yu4",
  # Deskari
  "de2 si1 ka3 rui4": "De2si1ka3rui4",
  # Aroden
  "e1 luo2 deng1": "E1luo2deng1",
  # Numeria
  "niu3 mei2 rui4 ya4": "Niu3mei2rui4ya4",
  # Baphomet
  "ba1 fu2 mie4": "Ba1fu2mie4",
  # Abadar
  "e1 ba1 da2": "E1ba1da2",
  # Kyonin
  "qiong2 zhu4": "Qiong2zhu4",
  # Absalom
  "an1 bu4 sa4 lang3": "An1bu4sa4lang3",
  # Savamelekh
  "sa4 wa3 mi3 lei1 ke4": "Sa4wa3mi3lei1ke4",
  # Cheliax
  "qie1 li4 ya4 ke4 si1": "Qie1li4ya4ke4si1",
  # Shelyn
  "xue3 lin2": "Xue3lin2",
  # Nerosyan
  "nie4 ruo4 xi1an1": "Nie4ruo4xi1an1",
  # Avistan
  "e1 wei2 si1 tan3": "E1wei2si1tan3",
  # Golarion
  "ge2 la1 li4 ang2": "Ge2la1li4ang2",
  # Hepzamirah
  "he4 pu3 ze2 mi2 la1": "He4pu3ze2mi2la1",
  # Pharasma
  "fa3 la1 si1 ma3": "Fa3la1si1ma3",
  # Andoran
  "an1duo1 an1": "An1duo1an1",
  # Lamashtu
  "la1 ma3 shen2 tu2": "La1ma3shen2tu2",
  # Hulrun
  "hu2 er3 lun2": "Hu2er3lun2",
  "hu2 er3 lun2 sha1 po4": "Hu2er3lun2sha1po4",
  # Urgathoa
  "wu1 jia1 suo1 e1": "Wu1jia1suo1e1",
  # Drezen
  "juan4 ze2 cheng2": "Juan4ze2cheng2",
  # Minagho
  "ming2 na4 gu3": "Ming2na4gu3",
  # Areelu
  "e1 rui4 lou4": "E1rui4lou4",
  "e1 rui4 lou4 wo4 lei1 shen2": "E1rui4lou4wo4lei1shen2",
  # Gorum
  "ge2 lu3 mu3": "Ge2lu3mu3",
  # Stonton
  "si1 tao2 dun4": "Si1tao2dun4",
  "si1 tao2 dun4 wei4 heng2": "Si1tao2dun4wei4heng2",
  # Elysium
  "ji2le4 jing4": "Ji2le4jing4",
  # EchoD
  "de2 si1 ka3 rui4 de5 hui2sheng1": "De2si1ka3rui4de5hui2sheng1",
  # Ciar
  "se4 er3": "Se4er3",
  "se4 er3 ke1 bei4 lun2": "Se4er3ke1bei4lun2",
  # Tar-Baphon
  "ta3 er3 ba1 feng1": "Ta3er3ba1feng1",
  # Azlant
  "e1 zi1 lan2te4": "E1zi1lan2te4",
  "e1 zi1 lan2te4 ren2": "E1zi1lan2te4ren2",
  # Osirion
  "ou1 xi1 li3ang2": "Ou1xi1li3ang2",
  "ou1 xi1 li3ang2 ren2": "Ou1xi1li3ang2ren2",
  # Mutasafen
  "mu4 ta3 sa1 fen1": "Mu4ta3sa1fen1",
  # Mephistopheles
  "mo4 fei1 si1 tuo1 fei1 li4 si1": "Mo4fei1si1tuo1fei1li4si1",
  # Razmir
  "la1 zi1 mi3 er3": "La1zi1mi3er3",
  # Khorramzadeh
  "huo4luan4 zha1 de2": "Huo4luan4zha1de2",
  # Dyra
  "dai4 la1": "Dai4la1",
  # Cayden Cailean
  "kai3 deng1 kai3 lin2": "Kai3deng1kai3lin2",
  "kai3 deng1": "Kai3deng1",
  # Jistka
  "ji4 si1 ka3": "Ji4si1ka3",
  "ji4 si1 ka3 ren2": "Ji4si1ka3ren2",
  # Darrazand
  "da2 ran2 zan4 de2": "Da2ran2zan4de2",
  # Jerribeth
  "jie2 rui4 bei4 si1": "Jie2rui4bei4si1",
  # Razmiran
  "la1 zi1 mi3lan2": "La1zi1mi3lan2",
  "la1 zi1 mi3lan2 ren2": "La1zi1mi3lan2ren2",
  # Zon-Kuthon
  "zuo3 en1 ku4 song1": "Zuo3en1ku4song1",
  # Galt
  "gao1 te4": "Gao1te4",
  # Socothbenoth
  "suo3 kou4 bei4 nuo4": "Suo3kou4bei4nuo4",
  # Lariel
  "la1 rui4er3": "La1rui4er3",
  # Shyka
  "se4 ka3": "Se4ka3",
  # Asmodeus
  "e1 si1 mo4 di2 si1": "E1si1mo4di2si1",
  # Terendelev
  "te4 lun2 di2 li4 fu2": "Te4lun2di2li4fu2",
  # Torag
  "tuo1 la1 ge2": "Tuo1la1ge2",
  # Norgorber
  "nuo4 ge2 ba1": "Nuo4ge2ba1",
  # Irabeth
  "yi1 la1 bei4 si1": "Yi1la1bei4si1",
  "ti2 la1 ba1 de2": "Ti2la1ba1de2",
  # Calistria
  "ka3 li2 si1 cui4": "Ka3li2si1cui4",
  # Nethys
  "nie4 xi1si1": "Nie4xi1si1",
  # Taldor
  "ta3 er3 duo1": "Ta3er3duo1",
  "ta3 er3 duo1 di4guo2": "Ta3er3duo1di4guo2",
  # River Kingdoms
  "he2 yu4 zhu1 guo2": "He2yu4zhu1guo2",
  # Garund
  "jia1 lun2 de2": "Jia1lun2de2",
  # Pitax
  "pi2 ta3 ke4 si1": "Pi2ta3ke4si1",
  # Anevia
  "an1ni1 wei2 ya4": "An1ni1wei2ya4",
  # Erastil
  "ai4 ruo4 si1 ti2": "Ai4ruo4si1ti2",
  # Xanthir
  "xian2 xi1er3": "Xian2xi1er3",
  # Gozreh
  "ge1 zi1 lei2": "Ge1zi1lei2",
  # Nulkineth
  "nu3 jin1 ni2si1": "Nu3jin1ni2si1",
  # Zanedra
  "zan4 ni1 de2 la1": "Zan4ni1de2la1",
  # Mireya
  "mi2 lei3 ya4": "Mi2lei3ya4",
  # Nidal
  "nai4 da2 er3": "Nai4da2er3",
  # Varisia
  "wa3 li3 xi1ya4": "Wa3li3xi1ya4",
  "wa3 li3 xi1ya4 ren2": "Wa3li3xi1ya4ren2",
  # Belkzen
  "bei4er3 ke4 ze2": "Bei4er3ke4ze2",
  "bei4er3 ke4 ze2 ling3": "Bei4er3ke4ze2ling3",
  # Sarenrae
  "suo1 en1 rui4": "Suo1en1rui4",
  # Horgus
  "huo4 er3 ge2 si1": "Huo4er3ge2si1",
  "huo4 er3 ge2 si1 ge2 wen1 mu3": "Huo4er3ge2si1ge2wen1mu3",
  # Nocticula
  "nuo4 ti2 ku4 la1": "Nuo4ti2ku4la1",
  # Joran
  "qiao2 lan2": "Qiao2lan2",
  "qiao2 lan2 wei4 heng2": "Qiao2lan2wei4heng2",
  # Ramien
  "la1 mi3 en1": "La1mi3en1",
  # Jannah
  "zhen1 na4": "Zhen1na4",
  # Brevoy
  "bu4lei2 wo4": "Bu4lei2wo4",
  # Abaddon
  "nai4 la4": "Nai4la4",
  # Jalmeray
  "jia1 mo2 rui4": "Jia1mo2rui4",
  # Irori
  "yi4 luo4 li3": "Yi4luo4li3",
  # Yaniel
  "yang2 ni1": "Yang2ni1",
  # Kabriri
  "ka3 bu4 li4 li4": "Ka3bu4li4li4",
  # Zacarius
  "ze2 ka3 liu2 si1": "Ze2ka3liu2si1",
  # Yaker
  "ya4 ke4": "Ya4ke4",
  "ya4 ke4 an1 kai3 er3": "Ya4ke4an1kai3er3",
  # Mivon
  "mi3 weng1": "Mi3weng1",
  # Markyll
  "ma3 kai3 er3": "Ma3kai3er3",
  # Curl
  "ke1 er3": "Ke1er3",
  # Kostchtchie
  "kuo4 shen2 ti2 ke4 ti2 kai3": "Kuo4shen2ti2ke4ti2kai3",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

def main():
  d = pyer.get_dictionary()

  with open('zhCN.json', encoding='utf-8') as f:
    data = json.load(f)

  strings = data['strings']
  MARKUP_TOKENS = {m for s in strings.values() for m in re.findall(r'\{[^}]+\}|<[^>]+>', s)}
  SPECIAL_TOKENS = MARKUP_TOKENS | pyer.special_tokens
  REPLACEMENTS = PROPER_NAME_REPLACEMENTS | PUNCTUATION_REPLACEMENTS

  for key, value in strings.items():
    strings[key] = add_pinyin(value, d, SPECIAL_TOKENS, REPLACEMENTS)

  with open('zhCN_with_pinyin.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=2)

def add_pinyin(string: str, d, SPECIAL_TOKENS, REPLACEMENTS: dict[str, str]):
  chinese, pinyin = pyer.get_segments_and_pinyin(string, d, SPECIAL_TOKENS)
  chinese = '\u200B'.join(chinese)
  pinyin = ' '.join(pinyin)
  if (chinese.count('\n') > 1):
    result = chinese + '\n\n' + pinyin
  else:
    result = chinese + '\n' + pinyin
  for old, new in REPLACEMENTS.items():
    result = result.replace(old, new)
  return result

if __name__ == "__main__":
  main()

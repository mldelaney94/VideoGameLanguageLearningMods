from polib import POEntry
import polib
import pinyiniser as pyer

# Formatting fixes: spaced-out punctuation/markup back to correct form
PUNCTUATION_REPLACEMENTS = dict(sorted({
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
}.items(), key=lambda kv: len(kv[0]), reverse=True))

# Template variable fixes: spaced-out game variables back to correct form
TEMPLATE_TOKENS = {
  "$(l.name)",
  "$(l.Name)",
  "$(l.sir)",
  "$(s.name)",
  "$+(l.honorific)",
  "$+(l.name)",
  "$(l.man)",
  "$(l.guy)",
  "$(s.man)",
  "$(s.guy)",
  "$(l.Race)",
  "$(l.Sir)",
  "$+(l.guy)",
  "$(l.race)",
  "$+(l.sir)",
  "$(l.race_plural)",
  "$+(l.race_plural)",
  "$(l.him)",
  "$(l.he)",
  "$(l.his)",
  "$+(l.he)",
  "$(l.sex)",
  "$(s.Name)",
  "$(scene.BroSis)",
  "$(scene.FatherMother)",
  "$(scene.TalkAbout)",
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
  # Coyote — 152 hits, two transliteration variants
  "kai3 yao1 di1": "Kai3yao1di1",
  "kai3 ao4 te4": "Kai3ao4te4",
  "kai3 yao1 ti2": "Kai3yao1ti2",
  # Telestrian — 146 hits, two transliteration variants
  "te4 li3 si1 tan3": "Te4li3si1tan3",
  "cui1 si1 tan3": "Cui1si1tan3",
  # Watts — 99 hits
  "hua2 ci2": "Hua2ci2",
  # Lynne — 60 hits
  "lin2 en1": "Lin2en1",
  # Kubota — 55 hits
  "ku4 bo1 ta3": "Ku4bo1ta3",
  # Baron — 42 hits
  "ba1 long2": "Ba1long2",
  # McKlusky — two transliteration variants
  "mai4ke4 lan2si1 ji1": "Mai4ke4lan2si1ji1",
  "mai4 ku4 lu2 si1 ji1": "Mai4ku4lu2si1ji1",
  # Johnny — 32 hits
  "qiang2 ni2": "Qiang2ni2",
  # Cherry — 19 hits
  "qie1 li4": "Qie1li4",
  # Aljernon — 19 hits
  "a1 er3 jie2 nong2": "A1'er3jie2nong2",
  # Dowd — 18 hits
  "duo1 de2": "Duo1de2",
  # Castle — 15 hits
  "ka3 suo3er3": "Ka3suo3er3",
  # Melinda — 14 hits
  "mei2 lin2 da2": "Mei2lin2da2",
  # Erik — 11 hits
  "ai1 li3 ke4": "Ai1li3ke4",
  # Silas — 11 hits
  "xi1 la1 si1": "Xi1la1si1",
  # Harlequin — two transliteration variants
  "ha1li4 kui2 yin1": "Ha1li4kui2yin1",
  "ha1 le4 kun1": "Ha1le4kun1",
  # Fosberg — 10 hits
  "fu2 si1 ba4 ge1": "Fu2si1ba4ge1",
  # Larry — 10 hits
  "niu3 La1rui4": "Niu3la1rui4",
  # Cherry Bomb — 9 hits
  "pang2 bo1": "Pang2bo1",
  # Buster — 8 hits
  "ba4 si1 te4": "Ba4si1te4",
  # Shannon — two transliteration variants
  "xiang1 nong2": "Xiang1nong2",
  "xia4 nong2": "Xia4nong2",
  # Valerie — 7 hits
  "wa3 lai2 li2": "Wa3lai2li2",
  # Jamal — 7 hits
  "jia3 ma3 er3": "Jia3ma3er3",
  # Maury — 6 hits
  "mao2 rui4": "Mao2rui4",
  # Vlad — 5 hits
  "fu2 la1 de2": "Fu2la1de2",
  # O'Malley — 5 hits
  "ao4 mai4 li4": "Ao4mai4li4",
  # Portland — 5 hits
  "bo1 te4 lan2": "Bo1te4lan2",
  # Armitage — 4 hits
  "a1 mi3 di4 ji1": "A1mi3di4ji1",
  # Donny — two transliteration variants
  "tang2 ni2": "Tang2ni2",
  "duo1 ni2": "Duo1ni2",
  # Manny — 3 hits
  "man4 ni2": "Man4ni2",
  # Kluwe variant — 3 hits
  "ke4 lu3 wei1": "Ke4lu3wei1",
  # Mossman — 2 hits
  "mo4 si1 man4": "Mo4si1man4",
  # Wells — 2 hits
  "wei1 er3 si1": "Wei1er3si1",
  # Lucy — 1 hit (usually auto-squished)
  "lu4 xi1": "Lu4xi1",
  # Walden (Lucy's surname) — 1 hit
  "wo4 deng1": "Wo4deng1",
  # Halfsky (Shannon's surname) — 1 hit
  "ha1 fu1 si1 ji1": "Ha1fu1si1ji1",
  # David — 1 hit (usually auto-squished)
  "da4 wei4": "Da4wei4",
  # Alexis — 1 hit
  "a1 li4 ke4 si1": "A1li4ke4si1",
  # Monica's surname — 1 hit
  "sa4 qie1 nuo4 fu1": "Sa4qie1nuo4fu1",
  # Holmes surname — 1 hit
  "huo4 ling2 si1": "Huo4ling2si1",
  # Forsberg surname
  "fu2 si1 bai3 ge2": "Fu2si1bai3ge2",
  # Paco
  "pa4 ke1": "Pa4ke1",
  # Gino
  "ji2 nuo4": "Ji2nuo4",
  # Jessica
  "jie2 xi1 ka3": "Jie2xi1ka3",
  # Jake
  "jie2ke4": "Jie2ke4",
  # Aguirre
  "a1 ji2 lei2": "A1ji2lei2",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

MISC_REPLACEMENTS = dict(sorted({
  # Spaced-out email
  "  E . Silverstar @ telestrian . ucas  ": "E.Silverstar@telestrian.ucas",
  # Aegis model number (runs after general space collapsing)
  "Mk . 1": "Mk.1",
  # Newsnet reference: / / xin1wen2wang3 / / → //xin1wen2wang3//
  "/ /": "//",
  "( 1 )": "(1)",
}.items(), key=lambda kv: len(kv[0]), reverse=True))

def main():
  d = pyer.get_dictionary()

  SPECIAL_TOKENS = MARKUP_TOKENS | TEMPLATE_TOKENS | pyer.special_tokens
  REPLACEMENTS = PROPER_NAME_REPLACEMENTS | PUNCTUATION_REPLACEMENTS | MISC_REPLACEMENTS

  seattle_pofile = add_pinyin('./materials/translations/zh_seattle_original.po', d, SPECIAL_TOKENS, REPLACEMENTS)
  deadmanswitch_pofile = add_pinyin('./materials/translations/zh_deadmanswitch_original.po', d, SPECIAL_TOKENS, REPLACEMENTS)

  seattle_pofile.save_as_mofile('zh_seattle.mo')
  deadmanswitch_pofile.save_as_mofile('zh_deadmanswitch.mo')

def add_pinyin(path_to_pofile, d, SPECIAL_TOKENS, REPLACEMENTS):
  pofile = polib.pofile(path_to_pofile)
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

if __name__ == "__main__":
  main()

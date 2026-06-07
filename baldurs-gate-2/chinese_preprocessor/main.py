import pinyiniser as pyer

# Infinity Engine substitution tokens as they appear in dialogue strings.
GAME_TOKENS = {'<CHARNAME>', '<BROTHERSISTER>', '<DAYANDMONTH>',
  '<DAYNIGHT>', '<DAYNIGHTALL>', '<GABBER>', '<GAMEDAY>', '<GAMEDAYS>',
  '<GIRLBOY>', '<HESHE>', '<HIMHER>', '<HISHER>', '<LADYLORD>', '<LEVEL>',
  '<MALEFEMALE>', '<MANWOMAN>', '<MONTH>', '<MONTHNAME>', '<DAY>', '<PLAYER1-6>',
  '<PRO_BROTHERSISTER>', '<PRO_CLASS>', '<PRO_GIRLBOY>', '<PRO_HESHE>',
  '<PRO_HIMHER>', '<PRO_HISHER>', '<PRO_LADYLORD>', '<PRO_MALEFEMALE>',
  '<PRO_MANWOMAN>', '<PRO_MASTERMISTRESS>', '<PRO_RACE>', '<PRO_SIRMAAM>',
  '<PRO_SONDAUGHTER>', '<RACE>', '<SIRMAAM>', '<SONDAUGHTER>', '<TM>', '<YEAR>',
  '<SPELLLEVEL>', '<WEAPONNAME>', '<SPECIALABILITYNAME>', '<ITEMCOST>',
  '<ITEMNAME>', '<DURATION>', '<HOUR>', '<PRO>', '<PLAYER1>', '<PLAYER2>',
  '<PLAYER3>', '<PLAYER4>', '<PLAYER5>', '<PLAYER6>', '<DAMAGER>', '<DAMAGEE>',
  '<AMOUNT>', '<TYPE>', '<FIGHTERTYPE>', '<MAGESCHOOL>', '<RESISTED>',
  '<SERVERVERSION>', '<CLIENTVERSION>', '<MINIMUM>', '<MAXIMUM>', '<script>',
  '<CLASS>', '<CurrentChapter>', '<HP>', '<EXPERIENCE>', '<NEXTLEVEL>',
  '<number>', '<DURATIONNOAND>', '<DOTS1>', '<DOTS2>', '<DOTS3>', '<DOTS4>',
  '<DOTS5>', '<EXPERIENCEAMOUNT>', '<TARGET>', '<CREATURE>', '<LEVELDIF>',
  '<losing>', '<battle>', '<RESOURCE>', '<PRO_HEHER>', '<AREA_NAME>',
  '<MISSING_CONTENT>', '<PERCENT>', '<COMPLETE>', '<TIME>', '<REMAINING>'
}

SPECIAL_TOKENS = GAME_TOKENS | pyer.special_tokens

def add_pinyin(strings: list[str], d, REPLACEMENTS: dict[str, str]):
  for string in strings:
    chinese, pinyin = pyer.get_segments_and_pinyin(string, d, SPECIAL_TOKENS)
    chinese = '\u200B'.join(chinese)
    pinyin = ' '.join(pinyin)
    if (chinese.count('\n') > 1):
      string = chinese + '\n\n' + pinyin
    else:
      string = chinese + '\n' + pinyin
    for old, new in REPLACEMENTS.items():
      string = string.replace(old, new)
  return strings

def main(lang, encoding):
  print(f"Processing {lang} with encoding {encoding}")

  tlk = read_tlk('materials/translations/dialog_zh.tlk')

  tlk.map_strings(

  )

  return None
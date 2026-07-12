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

def main():
  d = pyer.get_dictionary()

  with open('zhCN.json', encoding='utf-8') as f:
    data = json.load(f)

  strings = data['strings']
  MARKUP_TOKENS = {m for s in strings.values() for m in re.findall(r'\{[^}]+\}|<[^>]+>', s)}
  SPECIAL_TOKENS = MARKUP_TOKENS | pyer.special_tokens
  REPLACEMENTS = PUNCTUATION_REPLACEMENTS

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

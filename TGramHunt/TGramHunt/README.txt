========== |-_-| ==========
I Развёртывание проекта

0) Установите NodeJs (У меня стоит 16 версия)
Проверить версию ноды можно консольной командой "node -v"

1) нажмите на файл в TGramHunt "current folder cmd.bat"
это должно открыть cmd с переходом в текущую папку

2) введите "npm install"
команда заставит скачать пакеты указанные в package.json
пакеты хранятся в папке node_modules, в TGramHunt проекте

3) для начала работы вбейте

для одноразового билда
npm run build-prod  - собрать в prod моде
npm run build 		- собрать в dev моде

для пересборки при изменениях
npm run watch-prod	- пересобрать в prod моде
npm run watch 		- пересобрать в dev моде

пересборка отслеживает изменения в css и js файлах, но
если вы изменили конфиг webpack или добавили пакет в package.json,
то консоль нужно сбросить сочетанием клавиш Ctrl+C и вбить команду снова.

========== )0w0( ==========
II стили

Все стили перед билдингом импортируются в all_styles.scss.
Импортируйте без расширения т.е. вместо file_name.css будет file_name.
Можно использовать sass, если будет желание.

========== (-\/-) ==========
III скрипты

Добавлена поддержка typeScript.
Всё тоже самое, но типизированно.
При билде подтягивиются скрипты из папки js,
но скрипты во вложениях не подтягиваются.
Они должны быть импортированы скриптами из папки js,
либо добвьте путь до скрипта в webpack.config.js в entriesArray.

========== @0.0) ==========
IV Telegram Pre requests

1) В телеге найдите бота @BotFather
2) Создаёте нового командой /start
3) Далее вбиваем /newbot
4) вводим имя бота

========== (0.0@ ==========
V Telegram Pre requests

1) установите и зарегестрируйтесь в ngrok
не забудьте авторизоваться в приложении
https://dashboard.ngrok.com/get-started/setup
2) вбиваете
ngrok http "https://localhost:44327" --host-header="localhost:44327"
44327 это порт на котором открывается ваш сайт
3) запоминаем урл указанный в консольке ngrok (далее @ngrokUrl) 
2) В телеге отправялем @BotFather команду /setdomain
3) выбираем имя бота
4) вводим @ngrokUrl
5) в браузере переходим на @ngrokUrl

========== \ (-)_-\ ======
TypeScript Analyzer
https://marketplace.visualstudio.com/items?itemName=RichNewman.TypeScriptAnalyzerEslintPrettier

https://rich-newman.github.io/typescript-analyzer-eslint-prettier/walkthrough.html

После установки в VS
1) зайти в 
Tools/Options/TypeScript Analyzer
и выставить "Fix and format on Save" на true, "File extensions to lint" на "ts"

2) зайти в
Tools/TypeScript Analyzer (ESLint)/Edit Default Config
и у "@typescript-eslint/no-this-alias" выставить "off"

перезапустить VS

SonarLint
https://marketplace.visualstudio.com/items?itemName=SonarSource.SonarLintforVisualStudio2022
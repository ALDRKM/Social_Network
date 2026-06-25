$ErrorActionPreference = "Stop"
$base = "http://localhost:5043/api"
function J($o){ $o | ConvertTo-Json -Depth 6 -Compress }
function Post($u,$b){ Invoke-RestMethod -Uri "$base/$u" -Method Post -ContentType "application/json" -Body (J $b) }
function PostN($u){ Invoke-RestMethod -Uri "$base/$u" -Method Post }
function Put($u,$b){ Invoke-RestMethod -Uri "$base/$u" -Method Put -ContentType "application/json" -Body (J $b) }
function Get-($u){ Invoke-RestMethod -Uri "$base/$u" -Method Get }
$pw = "demo1234"

# ---------- Пользователи ----------
$users = @(
  @{ login="marina_sea";  email="marina@demo.app";  bio="Фотографирую море и закаты 🌊 Калининград"; ava="https://i.pravatar.cc/240?img=5"  }
  @{ login="denis_trip";  email="denis@demo.app";   bio="Горы, походы, путешествия ⛰ Пишу о маршрутах"; ava="https://i.pravatar.cc/240?img=12" }
  @{ login="alyona_art";  email="alyona@demo.app";  bio="Рисую, пишу заметки и немного мечтаю ✍"; ava="https://i.pravatar.cc/240?img=9"  }
  @{ login="igor_code";   email="igor@demo.app";    bio="Разработчик. Кофе ☕ и код день и ночь"; ava="https://i.pravatar.cc/240?img=33" }
  @{ login="katya_food";  email="katya@demo.app";   bio="Готовлю и делюсь рецептами 🍰"; ava="https://i.pravatar.cc/240?img=20" }
)
foreach($u in $users){
  $r = Post "auth/register" @{Login=$u.login;Email=$u.email;Password=$pw}
  $u.id = $r.id
  Put "user/$($u.id)" @{login=$u.login;bio=$u.bio;avatarUrl=$u.ava} | Out-Null
  Write-Host "user $($u.login) id=$($u.id)" -ForegroundColor Green
}
$id = @{}; foreach($u in $users){ $id[$u.login] = $u.id }

# ---------- Публикации ----------
function Photo($login,$seed,$text){ Post "post" @{UserId=$id[$login];Content=$text;Type=0;ImageUrl="https://picsum.photos/seed/$seed/700/700"} }
function Note($login,$text){ Post "post" @{UserId=$id[$login];Content=$text;Type=1} }

$posts = @{}
$posts.marina1 = (Photo "marina_sea" "balticsunset" "Закат на Балтике, не могу налюбоваться #море #закат #лето").id
$posts.marina2 = (Photo "marina_sea" "seafoam" "Утренний шторм #море #природа").id
$posts.marina3 = (Note  "marina_sea" "Сегодня весь день у воды. Море лечит #море #мысли").id

$posts.denis1 = (Photo "denis_trip" "mountainpeak" "Вершина взята! Вид того стоил #горы #путешествия").id
$posts.denis2 = (Note  "denis_trip" "Собрал список снаряжения для весеннего похода. Делюсь в комментариях #горы #поход").id
$posts.denis3 = (Photo "denis_trip" "foresttrail" "Тропа через лес #путешествия #природа").id

$posts.alyona1 = (Note  "alyona_art" "Дописала новую главу. Кажется, получилось живо #книги #творчество").id
$posts.alyona2 = (Photo "alyona_art" "sketchbook" "Мой сегодняшний скетч #арт #рисунок").id
$posts.alyona3 = (Note  "alyona_art" "Тихий вечер, чай и мысли #дзен #мысли").id

$posts.igor1 = (Note  "igor_code" "Запушил большую фичу в 4 утра. Сплю стоя #код #разработка").id
$posts.igor2 = (Photo "igor_code" "workspace" "Рабочее место мечты #код #сетап").id
$posts.igor3 = (Note  "igor_code" "Третий кофе за утро. Это норма? #кофе #код").id

$posts.katya1 = (Photo "katya_food" "tiramisu" "Тирамису по бабушкиному рецепту 🍰 #еда #десерт").id
$posts.katya2 = (Photo "katya_food" "breakfast" "Идеальный завтрак выходного дня #еда #завтрак").id
$posts.katya3 = (Note  "katya_food" "Рецепт настоящего борща — сохраняйте #еда #рецепты").id
Write-Host "posts created: $($posts.Count)" -ForegroundColor Green

# ---------- Подписки (полный граф) ----------
foreach($a in $users){ foreach($b in $users){ if($a.id -ne $b.id){ Post "subscription/follow" @{FollowerId=$a.id;FollowingId=$b.id} | Out-Null } } }
Write-Host "follow graph done" -ForegroundColor Green

# ---------- Лайки ----------
PostN "like/$($id.denis_trip)/$($posts.marina1)" | Out-Null
PostN "like/$($id.alyona_art)/$($posts.marina1)" | Out-Null
PostN "like/$($id.katya_food)/$($posts.marina1)" | Out-Null
PostN "like/$($id.igor_code)/$($posts.marina1)"  | Out-Null
PostN "like/$($id.marina_sea)/$($posts.denis1)"  | Out-Null
PostN "like/$($id.katya_food)/$($posts.denis1)"  | Out-Null
PostN "like/$($id.marina_sea)/$($posts.katya1)"  | Out-Null
PostN "like/$($id.alyona_art)/$($posts.katya1)"  | Out-Null
PostN "like/$($id.denis_trip)/$($posts.igor2)"   | Out-Null
PostN "like/$($id.marina_sea)/$($posts.alyona2)" | Out-Null
Write-Host "likes done" -ForegroundColor Green

# ---------- Комментарии + ответ ----------
$cm1 = Post "comment" @{UserId=$id.denis_trip;PostId=$posts.marina1;Content="Огонь! Где это снято?"}
Post "comment" @{UserId=$id.marina_sea;PostId=$posts.marina1;Content="Спасибо! Это Балтийск 🌊";ParentCommentId=$cm1.id} | Out-Null
Post "comment" @{UserId=$id.alyona_art;PostId=$posts.marina1;Content="Невероятно красиво"} | Out-Null
Post "comment" @{UserId=$id.igor_code;PostId=$posts.katya1;Content="Слюнки потекли :)"} | Out-Null
Post "comment" @{UserId=$id.katya_food;PostId=$posts.denis2;Content="Жду список!"} | Out-Null
Write-Host "comments done" -ForegroundColor Green

# ---------- Сохранённые ----------
PostN "savedpost/$($id.marina_sea)/$($posts.katya1)" | Out-Null
PostN "savedpost/$($id.marina_sea)/$($posts.denis1)" | Out-Null
PostN "savedpost/$($id.denis_trip)/$($posts.marina1)" | Out-Null

# ---------- Переписка Марина <-> Денис ----------
$chat = Post "chat" @{User1Id=$id.marina_sea;User2Id=$id.denis_trip}
$cid = $chat.id
$dialog = @(
  @($id.marina_sea, "Привет, Денис! Видела твоё фото с вершины — космос 🔥"),
  @($id.denis_trip, "Привет! Спасибо) Это Кавказ, ходили на выходных"),
  @($id.marina_sea, "Давно мечтаю туда. Сложный маршрут?"),
  @($id.denis_trip, "Средний. Новичкам норм с гидом. Могу скинуть трек"),
  @($id.marina_sea, "Было бы супер! А я тебе скину локации для фото у моря"),
  @($id.denis_trip, "Договорились 🤝")
)
foreach($m in $dialog){ Post "message" @{ChatId=$cid;SenderId=$m[0];Text=$m[1]} | Out-Null; Start-Sleep -Milliseconds 50 }
Write-Host "chat $cid seeded with $($dialog.Count) messages" -ForegroundColor Green

# ---------- ПРОВЕРКИ ----------
Write-Host "`n===== ПРОВЕРКИ =====" -ForegroundColor Cyan
$hist = Get- "message/$cid"
$fromM = ($hist | ? { $_.senderId -eq $id.marina_sea }).Count
$fromD = ($hist | ? { $_.senderId -eq $id.denis_trip }).Count
Write-Host "Переписка: всего=$($hist.Count), от Марины=$fromM, от Дениса=$fromD" -ForegroundColor Yellow
$feedM = Get- "post/feed/$($id.marina_sea)"
$others = ($feedM | ? { $_.userId -ne $id.marina_sea }).Count
Write-Host "Лента Марины: всего=$($feedM.Count), чужих постов=$others" -ForegroundColor Yellow
$tagSea = Get- "post/tag/море"
Write-Host "Поиск по #море: найдено=$($tagSea.Count)" -ForegroundColor Yellow
$tagFood = Get- "post/tag/еда"
Write-Host "Поиск по #еда: найдено=$($tagFood.Count)" -ForegroundColor Yellow
$chatsM = Get- "chat/user/$($id.marina_sea)"
Write-Host "Чаты Марины: $($chatsM.Count), последнее='$(($chatsM[0]).lastMessage.text)'" -ForegroundColor Yellow
$top = Get- "report/top-tags"
Write-Host "Топ тегов: $(($top | Select-Object -First 5 | % { '#'+$_.tag+'('+$_.count+')' }) -join ' ')" -ForegroundColor Yellow

Write-Host "`n===== АККАУНТЫ (пароль у всех: $pw) =====" -ForegroundColor Cyan
foreach($u in $users){ Write-Host ("  {0,-12} email={1,-18} id={2}" -f $u.login,$u.email,$u.id) }

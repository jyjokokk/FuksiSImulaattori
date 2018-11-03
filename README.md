# Ohjelmointi 1 - Harjoitustyon suunnitelma

## Pelihahmo - Fuksi
- PhysicsObject, ehka luodaan attribuuttina?
- Lisataan "kierto-ominaisuus" humalatilaan pohjautuen
- Luodaan Aliohjelmana

## Pistemittari
- Luodaan attribuuttina
- IntCounter
- Tutoriaalin mukaan
- Aliohjelmana

## Kentta
- LuoKentta() aliohjelmana
- CreateBorders()
- Camera.ZoomToLevel
- Luodaan BoundingArea(?) spawnAlue juomille, jonka rajat maaraataan Level.Bottom + 20, ja Level.Top + 20

## Juomat
- Luodaan aliohjelmalla LuoJuoma
- LuoJuoma():
- PhysicsObject.CreateStaticObject
- Luodaan satunnanvaraisesti alueelle spawnAlue
- Kutsutaan alussa kaksi kertaa, jotta juomia on aina kaksi kappaletta

## GameState
- Atribuutti, joka maarittaa onko peli kaynnissa vai ei
- Lisataan Keyboard.Listen escapille, joka muuttaa staten truesta falseen

## AloitaPeli - EHKA
- Alussa suoritettava ohjelma, joka kaynnistaa pelin asettamalla GameStaten trueksi

## Esteet
- Luodaan aliohjelmalla
- Tyyppi: PhysicsConstruct, jotta esteet pysyvat tietyn kokoisina ja tietyn etaisyyden paassa toisistaan
- Kutsutaan beginissa while loopilla, joka pyorii niin pitkaan kun GameState == true

## CollisionHandler
- Talle luodaan aliohjelma FuksiTormaa
- FuksiTormaa():
- Aina pelaajan osuessa juomaan, yksi juoma haviaa, lisataan pistemittariin piste ja luodaan uusi juoma, jotta ruudulla on aina kaksi juomaa.

# Myohemmin lisattavaa:

## Liikkuminen
- Attribuutteina esitettavat ylos ja sivulle liikkumistahdit Vectoreina
- Naita kaytetaan ohjaimien Vectoreina liikkumista maarattaessa
- Muokkautuu humalakertoimen mukaan

 ## Humalakerroin
 - Kasvaa pistemittarin tapaan, vaikuttaen liikkumis-Vectoireihin
 - Ehka lisataan kasvattamaan pisteiden kertymista?
 
## Katkaisuhoito
- Aliohjelma Katkaisu()
- Kutsuttavissa vain jos pistemaara on tarpeeksi korkea
  - (toteuteen if lauseella, esim if (pisteet > 10))
- Palauttaa Humalakertoimen maaritellylle tasolle


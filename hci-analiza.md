# HCI Analiza – [Ime i prezime studenta]

**Predmet:** Human–Computer Interaction (HCI)  
**Projekat:** Desktop klijent za Paperless-NGX  
**Tehnologija:** C# / .NET 8 (WPF)  
**Datum:** [unesi datum predaje]

---

## 1. Uvod

U ovom dokumentu predstavljena je HCI analiza i unapređenja korisničkog iskustva u okviru desktop klijenta za Paperless-NGX.  
Analiza obuhvata pet identifikovanih problema u web interfejsu, njihovo mapiranje na Nielsenove heuristike, kao i rješenja implementirana u desktop verziji aplikacije.  
Na kraju dokumenta opisana je dodatna funkcionalnost razvijena sa ciljem poboljšanja efikasnosti i kvaliteta interakcije korisnika sa sistemom.

---

## 2. Identifikovani problemi u web interfejsu

Za svaki problem potrebno je navesti sledeće informacije.  
Obavezno uključiti screenshotove web i desktop verzije, tabelu poređenja i kratki citat korisnika iz mini testiranja.

---

### Problem #1 – [Naziv problema]

**Opis problema:**  
Objasniti šta nije intuitivno ili efikasno u web interfejsu (npr. previše koraka za određenu akciju, nejasne ikone, preopterećen interfejs...).

**Screenshot web interfejsa:**  
(umetnuti sliku i označiti problem na slici)

**Nielsen heuristika:**  
Npr. H6 – Prepoznavanje umjesto pamćenja (Recognition rather than recall)  
Objasniti zbog čega je heuristika prekršena.

**Desktop rješenje:**  
Opisati kako je problem riješen u desktop verziji i priložiti screenshot.  
Navesti koliko su skraćeni koraci ili poboljšano razumijevanje korisničkog toka.

**Objašnjenje:**  
Zašto je novo rješenje bolje i kako adresira prepoznatu heuristiku.

**Mjerenje:**  
Mini test sa jednom osobom (ili više ako je moguće).  
Popuniti tabelu:

| Scenario | Web verzija | Desktop verzija |
|-----------|--------------|-----------------|
| [opis zadatka] | [vrijeme/koraci] | [vrijeme/koraci] |

---

*(isti format se ponavlja za Problem #2 – Problem #5)*

---

## 3. Dodatna funkcionalnost

Ova sekcija opisuje funkcionalnost koju ste dodatno implementirali radi poboljšanja korisničkog iskustva.

### Naziv funkcionalnosti:
(npr. Analitika dokumenata, Pretraga prirodnim jezikom, Offline mod, itd.)

### Zašto je ova funkcionalnost potrebna?
Kratko objasniti konkretan problem u postojećem sistemu koji ova funkcionalnost rješava.  
Npr. “Web interfejs ne omogućava pregled trendova uploadovanih dokumenata kroz vrijeme.”

### HCI principi i heuristike:
Navesti koje su Nielsen heuristike adresirane ovom funkcionalnošću (2–3 heuristike) i ukratko objasniti svaku.  
Primjer:  
- **H1 – Vidljivost stanja sistema (Visibility of system status):** Korisnik sada odmah vidi koliko dokumenata pripada kojem tipu.  
- **H7 – Fleksibilnost i efikasnost korištenja:** Napredni korisnici mogu brzo filtrirati klikom na grafikon.

### Implementacija:
Objasniti tehnički pristup i priložiti screenshot.  
Navesti korištene komponente, biblioteke i način interakcije (npr. klik, hover, drag & drop, glasovna komanda).  
Opisati ponašanje sistema i očekivani ishod.

### Testiranje:
Opisati kako je funkcionalnost testirana sa barem jednom osobom.  
U tabeli prikazati rezultate:

| Zadatak | Web verzija | Desktop verzija |
|----------|--------------|-----------------|
| [npr. „Pronađi mjesec sa najviše računa“] | [vrijeme/koraci] | [vrijeme/koraci] |

**Citat korisnika:**  
„[Kratak komentar o korisničkom iskustvu]“

### Tehnički detalji:
Navesti koje su biblioteke, tehnologije i dizajn obrasci korišteni.  
Primjer:  
- LiveCharts2 za vizualizaciju podataka  
- LINQ za agregaciju  
- MVVM arhitektura  
- Lazy loading za poboljšanje performansi

---

## 4. Zaključak

U zaključku ukratko sumirati glavna unapređenja u desktop klijentu u odnosu na web interfejs i opisati kako su predložena rješenja doprinijela boljem korisničkom iskustvu.  
Navedite koje su heuristike najviše kršene u originalnom interfejsu i na koji način su u vašoj implementaciji ispravljene.

---

## Napomene

- Dokument mora biti kreiran putem ovog markdown šablona, i predaje se u formatu **HCI_Analiza_Ime_Prezime.pdf** u korijen repozitorijuma.  
- Obim: 6–8 strana.  
- Svi screenshotovi moraju biti čitljivi i relevantni.  
- Sve tabele i slike moraju biti numerisane i kratko opisane.  
- Dokument mora sadržavati vaše ime i prezime na prvoj stranici.  

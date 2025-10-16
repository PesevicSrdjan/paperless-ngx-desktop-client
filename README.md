# Projekat: Desktop klijent za Paperless-NGX

**Predmet:** Human–Computer Interaction (HCI)  
**Tehnologija:** C# / .NET 8 (WPF)  
**Tip projekta:** Individualni projekat  
**Ukupan broj bodova:** 40 (+10 bonus bodova)

## Test podaci:

  **Username**: student_test
  **Password**: TestPass123
  **API Token**: 8aa5b12301e7a074559ec3e15b8c360dd97791a4

---

## 1. Opis zadatka

Zadatak je da student razvije **funkcionalan desktop klijent za Paperless-NGX** koji demonstrira **unaprijeđeno korisničko iskustvo** u odnosu na postojeći web interfejs, kroz **svjesnu primjenu principa interakcije čovjek–računar (HCI)**.

Projekat se sastoji iz dva dijela:

- **DIO A: Osnovne funkcionalnosti (20 bodova)**  
  Implementacija osnovnih funkcionalnosti potrebnih za rad sa dokumentima.

- **DIO B: HCI analiza i poboljšanja (20 bodova)**  
  Analiza postojećeg web interfejsa, identifikacija problema prema Nielsenovim heuristikama, dokumentovanje rješenja i implementacija jedne dodatne funkcionalnosti koja donosi novu vrijednost desktop klijentu.

Student može ostvariti do **10 bonus bodova** (do januarsko-februarskog roka zaključno sa 2. terminom) za implementaciju **druge dodatne funkcionalnosti** iz ponuđenog skupa.

---

## 2. DIO A – Osnovne funkcionalnosti (20 bodova)

Sve navedene funkcionalnosti moraju biti u potpunosti funkcionalne.  
Bez njih se projekat smatra neispravnim i ne može biti ocijenjen.

| # | Funkcionalnost | Opis | Bodovi |
|---|----------------|------|--------|
| 1 | Lista dokumenata | Prikaz svih dokumenata sa metapodacima (naziv, datum, tagovi, tip). Omogućiti paginaciju, kartični i tabelarni prikaz dokumenata . | 3 |
| 2 | Pretraga | Search box koji pretražuje dokumente po ključnim riječima. | 2.5 |
| 3 | Filtriranje | Filter po tagovima, korespondentu i tipu dokumenta. | 2.5 |
| 4 | Pregled dokumenta | Klik na dokument prikazuje preview i metapodatke. | 3 |
| 5 | Upload dokumenta | File picker ili drag & drop zona. Unos metapodataka i validacija. | 3 |
| 6 | Upravljanje tagovima | Kreiranje, uređivanje i brisanje tagova. | 2.5 |
| 7 | Podešavanja | Tema (svijetla/tamna, odabir boje iz color pickera) i jezik (srpski/engleski). Podešavanje se čuva lokalno. | 2 |
| 8 | Error handling | Jasne poruke o greškama, loading indikatori i validacija unosa. | 1.5 |

**Ukupno: 20 bodova**

---

## 3. DIO B – HCI analiza i poboljšanja (20 bodova)

Dio B se sastoji iz dva dijela:

- **B1. HCI dokumentacija (10 bodova)**  
- **B2. Dodatna funkcionalnost (10 bodova)**

---

### 3.1 B1 – HCI dokumentacija (10 bodova)

Student treba da identifikuje **5 problema u web interfejsu** Paperless-NGX sistema i da dokumentuje njihova rješenja koja je implementirao kroz svoj desktop klijent.

Dokument se predaje pod nazivom **HCI_Analiza_Ime_Prezime.pdf**, obima 6–8 strana.

Za svaki problem potrebno je navesti:

1. **Problem u web interfejsu (0.4 boda)**  
   - Screenshot sa označenim problemom  
   - Kratak opis i broj potrebnih koraka

2. **Prekršena heuristika (0.4 boda)**  
   - Označiti odgovarajuću Nielsen heuristiku (H1–H10)  
   - Ukratko objasniti zbog čega je prekršena

3. **Rješenje u desktop klijentu (0.6 bodova)**  
   - Screenshot desktop verzije  
   - Opis implementiranog rješenja  
   - Koliko je smanjen broj klikova ili vrijeme izvršavanja zadatka

4. **Objašnjenje (0.4 boda)**  
   - Zašto je novo rješenje bolje i kako adresira heuristiku

5. **Mjerenje (0.2 boda)**  
   - Mini test sa jednom osobom  
   - Tabela poređenja i jedan kratki citat

**Primjer tabele:**

| Scenario | Web verzija | Desktop verzija |
|-----------|--------------|-----------------|
| Otvori dokument i pronađi tagove | 25 s | 9 s |

Ukupno: **5 problema × 2 boda = 10 bodova**

---

### 3.2 B2 – Dodatna funkcionalnost (10 bodova)

Student bira **jednu funkcionalnost** iz ponuđenog skupa koja donosi novu vrijednost u odnosu na postojeći web interfejs.

Svaka funkcionalnost nosi 10 bodova i ima jasno definisane kriterijume.

#### Opcija A – Analitika dokumenata
Dashboard sa grafikonima i tag cloud prikazom.  
**Minimalno očekivanje:**  
- Pie chart po tipu dokumenta  
- Bar chart po mjesecima (gdje su mjesec i godina promjenljivi) 
- Tag cloud najčešće korištenih tagova  
- Klik na grafikon filtrira listu dokumenata  

#### Opcija B – Praćenje promjena (Watcher)
Automatsko ažuriranje liste dokumenata kada se promjene dese na serveru.  
**Minimalno očekivanje:**  
- Polling svakih X sekundi (SignalR, Webhook opciono)  
- Glatko ažuriranje bez gubitka fokusa  
- Status indikator „Last sync: 2 min ago“

#### Opcija C – Pretraga prirodnim jezikom
Omogućava unos u prirodnom jeziku (npr. „nađi račune iz juna“).  
**Minimalno očekivanje:**  
- Parser koji prepoznaje ključne pojmove (tag, datum, tip)  
- Prevod natural language upita u API filtere  
- Prikaz interpretiranog upita (npr. „tag=računi, mjesec=juni“)

#### Opcija D – Offline mod
Rad bez internet konekcije uz lokalno keširanje dokumenata.  
**Minimalno očekivanje:**  
- Dokumenti keširani lokalno (SQLite ili file sistem)  
- Pregled dostupnih dokumenata offline  
- Status indikator online/offline

#### Opcija E – Dodatni modaliteti interakcije
Uvođenje više načina interakcije: glas, tastatura i kontekstualni meni.  
**Minimalno očekivanje:**  
- 3 glasovne komande (npr. Upload, Search, Refresh)  
- 5 tastaturnih prečica (Ctrl+N, Ctrl+F, F5...)  
- 3 akcije u desnom kliku (Open, Rename, Delete)

#### Opcija F – Personalizacija interfejsa
Omogućavanje prilagođavanja izgleda i rasporeda elemenata.  
**Minimalno očekivanje:**  
- Drag & drop raspored panela  
- Sačuvane pretrage (saved searches)  
- Reset na podrazumijevani izgled

Sve funkcionalnosti su ujednačene po težini i vremenu izrade.

---

### Način ocjenjivanja dodatne funkcionalnosti

| Kriterijum | Opis | Bodovi |
|-------------|------|--------|
| Tehnička implementacija | Funkcionalnost radi i može se demonstrirati. | 5 |
| Korisna vrijednost | Donosi novu vrijednost koju web interfejs nema. | 2 |
| HCI objašnjenje | Obrazloženo u HCI_Analiza_Ime_Prezime.pdf zašto funkcionalnost poboljšava UX. | 2 |
| Kvalitet izvršenja | Uredan UI, validacija i rukovanje greškama. | 1 |
| **Ukupno** |  | **10** |

---

## 4. HCI dokumentacija – šablon

Student mora predati dokument **HCI_Analiza_Ime_Prezime.pdf** koji sadrži:

1. Uvod (cilj projekta i kratko objašnjenje pristupa)  
2. Pet identifikovanih problema sa rješenjima (vidi sekciju 3.1)  
3. Sekciju 6 – Dodatna funkcionalnost:  
   - Zašto je potrebna  
   - HCI principi i heuristike koje adresira  
   - Implementacija i interakcija (sa slikama)  
   - Testiranje (poređenje Web vs Desktop)  
   - Tehnički detalji (biblioteke, izazovi, rješenja)

Preporučeni obim: 6–8 strana.

---

## 5. Odbrana projekta

Svaki student ima **10 minuta** za odbranu projekta.

**Struktura odbrane:** 
1. Demonstracija osnovnih funkcionalnosti (3 min)  
2. Demonstracija dodatne funkcionalnosti (3–4 min)  
4. Objašnjenje HCI principa i jednog identifikovanog problema (3 min)

**Aplikacija mora da se pokrene lokalno** i sve funkcionalnosti moraju biti vidljive tokom demonstracije.

---

## 6. Bonus bodovi

Student može ostvariti dodatnih **10 bodova** implementacijom **druge dodatne funkcionalnosti** iz skupa A–F.  
Bonus je opcion i ne utiče negativno na ocjenu ako nije urađen, moguće ga je ostvariti zaključno sa 2. terminom januarsko-februarskog roka.

---

## 7. Gubitak bodova

| Problem | Oduzimanje |
|----------|-------------|
| Aplikacija se ne pokreće ili puca tokom izvršavanja | -10 |
| Osnovne funkcionalnosti ne rade | -5 do -15 |
| HCI_Analiza_Ime_Prezime.pdf ne postoji | -10 |
| UI nije responzivan | -5 |
| Loša struktura koda | -3 |
| Istorija commita ne slijedi logičan razvoj projekta ili je nepostojeća | -10 |
| Plagijat | 0 bodova (projekat se poništava) |

---

## 8. Ocjenjivanje

| Komponenta | Bodovi |
|-------------|---------|
| DIO A: Osnovne funkcionalnosti | 20 |
| DIO B1: HCI dokumentacija | 10 |
| DIO B2: Dodatna funkcionalnost | 10 |
| Bonus funkcionalnost | +10 |
| **Ukupno** | **40 (+10)** |
---

## 9. Način predaje

Student predaje sljedeće materijale:
- Izvorni kod projekta (GitHub repozitorijum i zip projekat sa svim fajlovima na Moodle kurs predmeta)  
- Dokument **HCI_Analiza_Ime_Prezime.pdf**   
- Sve potrebno da aplikacija može biti pokrenuta i demonstrirana bez dodatne konfiguracije

Rok za predaju je **7 dana prije definisanog termina ispita**, termin odbrane će biti blagovremeno objavljen.

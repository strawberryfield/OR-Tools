# Giocare con i file .s

### Scherzosa analisi dei file .s 

Originariamente pubblicato su [TrainSimHobby](http://www.trainsimhobby.net/forum/viewtopic.php?t=10514)


## Introduzione

Fosse Sandro intitolerebbe questo post "Indiana Jones e il Bounding Box"; in effetti la storia che vado a raccontare ha parecchio dell'avventuroso.
Per indole io sono uno che i problemi invece che affrontarli trova sempre il modo di scansarli, ma questa volta le cose sono andate un po' diversamente...

Iniziamo con il descrivere il problema: creare un file .sd per i miei modelli 3d creati con gmax (che a differenza di altri programmi "dedicati" non lo crea automaticamente).

Una soluzione abbastanza comoda era utilizzare "Shape File Manager" che crea un file dove bisogna solo impostare se ci sono texture alternative (neve, notte, stagioni...) cosa che il programma non può sapere, e il "bounding box" che invece il programma potrebbe ricavare dal modello.
L'unico sistema per evitare di calcolarsi l'ingombro dell'oggetto è quello di aprire il modello con "Shape Viewer", far apparire la finestra con i dati calcolati del bounding box e riportarli manualmente (e nell'ordine giusto, che è diverso da quello con cui li presenta shape viewer!) all'interno del file .sd
Ce n'è fin troppo per creare inutili e fastidiosi errori.

Dò una rapida occhiata al contenuto del file .s che contiene il modello della stazioncina che stavo realizzando e quasi all'inizio trovo questa interessante sequenza 

```
      points ( 52
         point ( 6.5 0.103621 11.5 )
         point ( 6.5 11 11.5 )
         point ( 6.5 11 -11.5 )
         point ( 6.5 0.103621 -11.5 )
         point ( -6.5 0.103621 11.5 )
         point ( -6.5 0.10362 -11.5 )
         point ( -6.5 11 -11.5 )
         point ( -6.5 11 11.5 )
         point ( 6.51 4.9 1.5 )
         point ( 6.51 5.38115 1.5 )
         point ( 6.51 5.38115 -1.5 )
         point ( 6.51 4.9 -1.5 )
         point ( -1.5 4.63746 11.55 )
         point ( -1.5 5.29 11.55 )
         point ( 1.5 5.29 11.55 )
         point ( 1.5 4.63746 11.55 )
         point ( 1.5 4.73199 -11.55 )
         point ( 1.5 5.29 -11.55 )
         point ( -1.5 5.29 -11.55 )
         point ( -1.5 4.73199 -11.55 )
         point ( -7 10.9 -12.05 )
         point ( -7 11.1 -12.05 )
         point ( -7 11.1 12.05 )
         point ( -7 10.9 12.05 )
         point ( 7 10.9 -12.05 )
         point ( 7 10.9 12.05 )
         point ( 7 11.1 12.05 )
         point ( 7 11.1 -12.05 )
         point ( 0 14.3 -5.05 )
         point ( 0 14.3 5.05 )
         point ( -7 11.5 12.05 )
         point ( -7 11.5 -12.05 )
         point ( 7 11.5 12.05 )
         point ( 7 11.5 -12.05 )
         point ( 0 14.7 5.05 )
         point ( 0 14.7 -5.05 )
         point ( -6.45 0.103621 11.5 )
         point ( -6.45 0.103621 -11.5 )
         point ( -6.45 11 -11.5 )
         point ( -6.45 11 11.5 )
         point ( 6.45 0.103621 11.5 )
         point ( 6.45 11 11.5 )
         point ( 6.45 11 -11.5 )
         point ( 6.45 0.103621 -11.5 )
         point ( 6.5 0.103621 -11.45 )
         point ( 6.5 11 -11.45 )
         point ( -6.5 11 -11.45 )
         point ( -6.5 0.10362 -11.45 )
         point ( 6.5 11 11.45 )
         point ( 6.5 0.103621 11.45 )
         point ( -6.5 0.103621 11.45 )
         point ( -6.5 11 11.45 )
      )   
```

Riconosco facilmente le coordinate dei punti che compongono il mio modello: se prendo i valori massimo e minimo su ogni coordinata ottengo il boundung box.
Butto giù qualche riga di codice ed ecco un bel programmino che calcola i valori esattamente come shape viewer, ma scrive anche il file .sd: ah, che gran comodità!

In un messaggio accenno alla cosa e mi viene chiesto di renderlo pubblico perchè può essere utile; impacchetto il tutto e lo mando per la pubblicazione.
Si fa subito avanti Vittorio "robitaillefan" per testarlo: che grande onore!
A ruota anche un altro modellatore, notoriamente "scarso":smiley: si mette a disposizione per il test.

Succede che, mentre Vittorio non riscontra alcun problema, il "modellatore scarso :smiley:" si lagna perchè non riesce ad ottenere risultati corretti.
In effetti provo con alcuni oggetti creati da lui e vengono fuori dei numeri "strani", mentre shape viewer riporta sempre i valori corretti.
Inizialmente abbiamo pensato fosse il programma di modellazione, ma poi abbiamo scoperto che tutto dipendeva dalla diversa gestione dei pivot: centrati all'origine degli assi i miei, al centro degli oggetti i suoi.
Da qualche parte nel file .s ci dovevano essere gli offset dei pivot: in effetti era anche abbastanza semplice individuarli, ma non era affatto chiaro come associare i singoli punti al loro offset.

Non esiste, o perlomeno io non sono stato in grado di trovarlo, un documento che spieghi come sono fatti questi files e quindi bisognava fare delle indagini più approfondite.
La prima idea era quella di dire: "che si arrangi, impari da Vittorio come si fa! il programma me lo tengo per me e buonanotte".
Ma Guido è un amico, non è affatto vero che sia "scarso" come modellatore, mi ha anche insegnato tanti trucchetti... insomma questa volta potevo fare qualcosa che gli fosse utile: perchè non provarci?
Si scoprirà in seguito che anche degli oggetti fatti da Vittorio non avrebbero dato i risultati corretti con la versione originale del programma, mi riferisco ad un modello della E428 da oltre 11000 poligoni che ho utilizzato per "mettere alla frusta" il programma.

Così invece di fare "La Settimana Enigmistica" ho cercato di risolvere questo enigma.
Ne è venuta fuori una interessante analisi della struttura dei file .s, che vi presenterò nei prossimi post, ed ho anche trovato degli interessanti documenti, che presenterò al momento oppurtuno, che mostreranno come si può "giocare" con gli oggetti modellati. 


## I primi commenti

#### da RobitailleFan » 30 ago 2012, 22:27

Ottimo, mi interessa questo topic, c'è sempre qualcosa da imparare.

P.S. 1 Io il tuo programma lo ho provato con costruzioni da G-Max, per l'appunto con il pivot point allo zero, e forse è per quel motivo che non ho avuto problemi. E, comunque, per creare una locomotiva a casse articolate ho dovuto editare manualmente il file .sd, mentre per quella tutta un pezzo non ho avuto alcun problema.

P.S. 2 La E.428 che hai utilizzato è stata creata non con G-Max, ma con Train Sim Modeller: considerando che i due programmi non funzionano in modo eguale forse alla fine il problema sta in quelle differenze tipiche dei programmi.


#### da timetable57 » 31 ago 2012, 0:45

Al punto 0,0,0, i pivot con Max, li metto solo all'oggetto MAIN, così consigliano alcuni autori.. Poi mi vien meglio con gli oggetti da trasformare per R3D. Mi vengono meglio le simmetrie.


## Le matrici e i vertici

Vi devo confessare che ho preparato questa serie di post all'inizio di agosto, prima che il programma venisse pubblicato, ma ero intenzionato ad attendere la pubblicazione del programma e quindi la conferma della correttezza di quanto veniva esposto.
Per motivi ancora sconosciuti è finito in download il programma originale (quello che funziona solo a me a Vittorio).

In attesa che Guido "faccia le pulci" alla versione finale possiamo procedere perchè quello che vedremo è stato in ogni caso "testato"...

Eravamo alla ricerca della posizione dei pivot dei vari componenti il modello.
Scorrendo il file .s si trova una sezione che pare proprio fare al caso nostro; l'esempio viene da un file di Guido tratto dalla Ferrovia delle Dolomiti. 

```
      matrices ( 3
         matrix MAIN ( 1 0 0 0 1 0 0 0 1 0 0 0 )
         matrix ROOF ( 1 0 0 0 1 0 0 0 1 0 3.65301 0 )
         matrix SNOW ( 1 0 0 0 1 0 0 0 1 0 0.302592 0 )
      )
```

Vediamo i nomi dei vari componenti e negli ultimi tre valori di ogni riga ci sono gli scostamenti lungo i tre assi; gli altri nove valori al momento non ci interessano.
Si nota però che la neve è ad una quota inferiore al tetto: evidentemente i tre oggetti sono collegati a cascata e quindi la quota è relativa al livello precedente della struttura.
Ma allora dove sta la descrizione della struttura?

Scorrendo il file arriviamo ad una riga dal nome inequivocabile 

```
   hierarchy ( 3 -1 0 1 )
```
 
 Cerchiamo di capire come funziona.
Il primo parametro ci dice quante matrici abbiamo, in questo caso solo 3.
Poi ci sono nell'ordine il livello cui è agganciata ogni matrice (faccio notare che in tutti gli elenchi si inizia a contare da 0 e non da 1); il valore -1 è un "tappo" che segna la fine della catena.
Nel caso in esempio il MAIN non discende da nulla, il ROOF dipende dalla matrice in posizione 0 cioè il MAIN, mentre SNOW dipende dalla matrice in posizione 1 cioè ROOF.

Con questi dati a disposizione possiamo calcolare le coordinate assolute (ciè relative all'origine degli assi) di ogni punto e poi calcolare minimi e massimi per avere i punti estremi e quindi il bounding box.

Una cosa però non la sappiamo ancora: a quale matrice appartiene un punto?
Questa informazione non è presente, almeno in questa forma, all'interno del file.
Bisogna considerare che il file è fatto per rendere il più rapido possibile il rendering, non certo per farci dei calcoli sopra: la presenza di dati precalcolati come il bounding box serve proprio per evitare alla macchina calcoli ripetitivi durante l'esecuzione del gioco.

Una cosa che interessa molto alla macchina sono i vertici dei poligoni che contengono al loro interno informazioni sulla posizione (i punti che già conosciamo), i mapping delle texture ed i vettori delle normali.
Esaminiamo quindi l'elenco dei vertici 

```
      vertices ( 78
         vertex ( 00000000 0 19 ff969696 ff969696
            vertex_uvs ( 1 0 )
         )
         vertex ( 00000000 1 19 ff969696 ff969696
            vertex_uvs ( 1 1 )
         )
         vertex ( 00000000 2 19 ff969696 ff969696
            vertex_uvs ( 1 2 )
         )
         vertex ( 00000000 3 19 ff969696 ff969696
            vertex_uvs ( 1 3 )
         )
         vertex ( 00000000 1 18 ff969696 ff969696
            vertex_uvs ( 1 4 )
         )
         ...
      )
```

Della riga "vertex" a noi interessa il secondo parametro che è proprio il numero d'ordine del punto con le coordinate del vertice.
Notare che un punto può essere utilizzato per più vertici se in comune a più poligoni (come nell'ultima riga dell'esempio).

Subito sotto all'elenco dei vertici troviamo questa tabella: 

```
      vertex_sets ( 3
         vertex_set ( 0 0 18 )
         vertex_set ( 1 18 34 )
         vertex_set ( 2 52 26 )
      )
```

Il primo parametro di ogni "vertex_set" sembra tanto il numero della matrice, il secondo è il primo vertice del set e l'ultimo il numero dei vertici che lo compongono.

Mi sembra di aver trovato la quadra: cerco il numero del punto nell'elenco dei vertci, prendo il numero di vertice in cui l'ho trovato e controllo a quale vertex_set appartiene.
A questo punto so a quale matrice appartiene, qundi sommo tutti gli offset di tutti i livelli di struttura da cui dipende e quindi ottengo i valori assoluti dei punti.
Trovati i valori assoluti il calcolo del bounding box è banale (la ricerca dei valori minimo e massimo).

Il programma così modificato da risultati corretti con tutti gli oggetti della FDD e, mi dice Guido, anche quelli della Parenzana.
Ma lanciandolo su una sua "locomotivetta" appare un errore in cui si dice che non trova un elemento nella tabella delle matrici. 

## I vertex_state

Ci eravamo lasciati con il dover risolvere il mistero della matrice mancante; per fare questo utilizziamo un oggetto un po' più complesso: una carrozza "Corbellini" modellata da Renzo Grassi che presenta il medesimo "problema" lamentato da Guido.

Iniziamo con il controllare la tabella delle matrici 

```
      matrices ( 10
         matrix CASSA ( 1 0 0 0 1 0 0 0 1 0 0 0 )
         matrix Bogie1 ( 1 0 0 0 1 0 0 0 1 0 0.655 6.2 )
         matrix Wheels11 ( 1 0 0 0 1 0 0 0 1 0 -0.087 1.27132 )
         matrix Wheels12 ( 1 0 0 0 1 0 0 0 1 0 -0.087 -1.224 )
         matrix Bogie1interno ( 1 0 0 0 1 0 0 0 1 0 0 -4.76837e-007 )
         matrix Bogie2 ( 1 0 0 0 1 0 0 0 1 0 0.655 -6.2 )
         matrix Wheels22 ( 1 0 0 0 1 0 0 0 1 0 -0.087 -1.20703 )
         matrix Wheels21 ( 1 0 0 0 1 0 0 0 1 0 -0.087 1.28797 )
         matrix Bogie2interno ( 1 0 0 0 1 0 0 0 1 0 0 0 )
         matrix maniglione ( 1 0 0 0 1 0 0 0 1 -0.00230217 1.83074 0.00377583 )
      )
```

Ci sono 10 matrici (numerate, ricordo, da 0 a 9)
Diamo anche uno sguardo alla gerarchia 

```
   hierarchy ( 10 -1 0 1 1 1 0 5 5 5 0 )
```

Anche qui si fa riferimento a 10 matrici; controlliamo i vertex_sets 

```
      vertex_sets ( 11
         vertex_set ( 0 0 952 )
         vertex_set ( 1 952 500 )
         vertex_set ( 2 1452 110 )
         vertex_set ( 3 1562 140 )
         vertex_set ( 4 1702 140 )
         vertex_set ( 5 1842 8 )
         vertex_set ( 6 1850 110 )
         vertex_set ( 7 1960 140 )
         vertex_set ( 8 2100 140 )
         vertex_set ( 9 2240 8 )
         vertex_set ( 10 2248 100 )
      )
```

Sono 11, uno in più rispetto alle matrici.

L'ultima riga fa riferimento ad una ipotetica matrice 10 che non esiste!
Ciò significa che quel numero non è il numero della matrice, come avevo ipotizzato, ma si riferisce ad altro.

Cercando nella zona delle matrici trovo un'altra tabella di 11 elementi

```
      vtx_states ( 11
         vtx_state ( 00000000 0 -5 0 00000002 )
         vtx_state ( 00000000 0 -8 0 00000002 )
         vtx_state ( 00000000 1 -5 0 00000002 )
         vtx_state ( 00000000 2 -5 0 00000002 )
         vtx_state ( 00000000 3 -5 0 00000002 )
         vtx_state ( 00000000 4 -5 0 00000002 )
         vtx_state ( 00000000 5 -5 0 00000002 )
         vtx_state ( 00000000 6 -5 0 00000002 )
         vtx_state ( 00000000 7 -5 0 00000002 )
         vtx_state ( 00000000 8 -5 0 00000002 )
         vtx_state ( 00000000 9 -5 0 00000002 )
      )
```

L'anello di congiunzione sembra essere proprio il secondo valore di ogni riga; infatti i valori delle prime due righe sono uguali e fanno entrambi riferimento al main.

Vale la pena di fare una prova leggendo questa tabella per ricavare il numero della matrice da quello del vertex_set.
I risultati sono lusinghieri: anche la carrozza "Corbellini" adesso viene calcolata correttamente.

Ma non è ancora ora di cantare vittoria, in tutto questo abbiamo trascurato due cose importanti non presenti in questo modello: i lod ed i subobject: vertici e vertex set sono proprio all'interno di queste sezioni.

Oggetti senza LOD e SubObject non creano problemi, ma le mie stazioncine, che usano i subobject, non passano più... 

## Altri commenti

#### da leo_1982 » 6 set 2012, 14:01

Ciao Roberto,

una piccola domandina, perché ti fai tanto lavoro per creare la bounding Box di tutti gli oggetti che crei nella .sd?

La Bounding Box serve alle seguenti informazioni:

Modelli statici:
-Definire dove passa il fumo oppure il vapore di locomotive termiche e dove passa la piogga e dove nò, perciò fa senso mettere una Bounding Box nella .sd di Modelli come Ponti sovrastanti i binari, portali di gallerie, pensiline. In Questo caso però la tua Bounding Box creata col metodo sopra descritto non adempie lo scopo, perché impedische al fumo anche di propagarsi sotto al limite da té desiderato, per questi modelli comunque la maniera più precisa é quella di creare questo valore manualmente.

Modelli rotabili:
-riconoscere collisioni tra i differenti rotabili e definire le distanze tra i modelli in una composizione.

Per tutti i modelli non rotabili e non appartenenti ai casi sopra citati io consiglio vivamente di cancellare completamente la bounding box dalla .sd, il valore se presente non porta a nessun vantaggio ma consuma rissorse del programma perché comunque il TS ne tiene conto e lo calcola. In un Tile con ca 1000 Oggetti sulla rampa del Gottardo abbiamo "guadagnato" ca un 10-15 % in più di Frames per secondo cancellando tutti i Bounding-Box inutili!

Come sempre potete naturalmente mettere in discussione il mio sapere e saperne di più, ma vi dico onestamente quel che abbiamo pututo constatare con test fatti seriamente su diversi PC... Dato che per il mio sistema di catenaria statica ho anch'io programmato un tool che crea i rispettivi .sd so di che parlo e non sparo giusto m... a vanvera

Cari saluti dalla svizzera, 


#### da strawberryfield » 6 set 2012, 17:08

Diciamo che la cosa che mi interessava di più era creare un file .eng per gli oggetti (tutti statici) del quale mi interessava solo il dato di lunghezza.
Il tutto per poi convertire il file per Rail3D.

Ovviamente quel valore viene calcolato dal bounding box, e quindi riutilizziamo quanto abbiamo lungo la strada...

Che il bounding box sia perfettamente inutile in certi casi lo sapevo, ma come vedi lo scopo era ben altro; poi come dicevo questa cosa ha fatto le veci degli "incroci obbligati" o del "Bartezzaghi"

Grazie comunque per le info sull'utilità del bonding box 


#### da timetable57 » 6 set 2012, 17:08

Ciao Leo, scusa ma non ho capito il tuo intervento.

Se alcuni oggetti non necessitano di BB questo non vuol dire che non sia necessario crearlo per altri. Sennò cosa ci starebbe a fare?
Io non ricordo esattamente con cosa modelli (Crafter/Canvas?). Questo programma genera automaticamente i file sd così come TSM e comprensivi di BB (non sempre affidabile.

Se leggi tutto il topic vedrai che questo programmino è stato scritto da Roberto per chi usa G-Max/3D Studio Max che in questo forum pian piano stan diventando la maggioranza.
Inoltre questo programmino , creando i BB, ma soprattutto gli .eng con le misure corrette, serve a chi come me e Roberto costruisce modelli complessi per Rail3D.


#### da RobitailleFan » 6 set 2012, 18:49

Attenzione: credo che Leo intenda dire che non è il file .sd che non serve, ma parte del suo contenuto. Per la precisione, qualsiasi oggetto in Train Simulator DEVE avere un file .sd, altrimenti TS si inca@@za. Se poi all'interno del file .sd non sono specificate le dimensioni dell'oggetto stesso e il tutto funziona ugualmente questo è un altro paio di maniche.
Ovvio che per certi oggetti le dimensioni sono obbligatorie.


#### da timetable57 » 6 set 2012, 23:34

Certo Leo si è spiegato benissimo.

Se non vado errato però il topic non verte tanto sulla creazione di un file .sd semplice. Per quello basta e avanza SFM.
Il problema indicato da Roberto è invece proprio quello di estrapolare le misure effettive di un 3D che poi servono sia per i BB sia per poter lavorare su altri Sim


#### da leo_1982 » 8 set 2012, 22:23

Salve a tutti,

certo che il File .sd ci vuole, soltanto la Bounding Box può essere tralasciata...

Per quanto riguarda il problema di Roberto: La matrice descrive nelle ultime 3 posizioni le coordinate rispettive alla sua "mamma", le cifre davanti (i 0 e 1) invece descrivono la gerarchia del Modello, se tutti tranne il main hanno le stesse cifre se non erro significa che sono tutti dei "child" diretti del main, però non ho mai analizzato la questione più di quel tanto... Con Modelli complessi (child di child) invece sarebbe neccessario analizzare anche questi casi perché li sarebbero le coordinate di più suboggetti da sommare in confronto con il main. 


#### da strawberryfield » 8 set 2012, 22:55

**leo_1982 ha scritto:**

*Per quanto riguarda il problema di Roberto: La matrice descrive nelle ultime 3 posizioni le coordinate rispettive alla sua "mamma", le cifre davanti (i 0 e 1) invece descrivono la gerarchia del Modello,*

Assolutamente no.
La gerarchia è gestita dalla riga Hierarchy (lo dice la parola stessa)

I nove numeri descrivono una matrice 3x3 il cui utilizzo verrà svelato a breve.


## LODs e subobjects

Certo le cose cominciavano a mettersi male, il lavoro da fare era aumentato, i tempi di calcolo anche, ma soprattutto il programma non assolveva più allo scopo per il quale era stato creato (convertire le mie casine).

Sfortunatamente per me, la definizione dei vertici è ripetuta per ogni lod, ma il fatto che il lod più definito (cioè quello con la minima distanza limite) sia sempre il primo aiuta non poco: tutti i lod successivi possono essere ignorati.
Quello che invece "infastidisce" di più e il fatto che lo stesso "spezzattamento" avviene nei subobjects e qui non abbiamo modo di aggirare il problema.

Il vertex_set fa sempre riferimento alla numerazione "locale" al subobject dei vertici e quindi ogni subobject va trattato a se. Ma i punti sono tutti in un unico blocco.
Mi è toccato fare una rinumerazione dei vertici e dei loro riferimenti nel vertex_set riducendo anch'essi ad un blocco unico.

Bene, adesso le mie stazioncine potevano essere elaborate correttamente e, speravo, anche la locomotiva di Guido.

Ma così non è stato: nel file .s della locomotiva non venivano riconosciute alcune matrici.
E perchè mai? Avevo un terribile presentimento...
Guardiamo come sono fatte le matrici di quel modello.

```
	matrices ( 35
         matrix MAIN ( 1 0 0 0 1 0 0 0 1 0 0 0 )
         matrix CALDAIA ( 1 0 0 0 1 0 0 0 1 0 2.00571 1.13628 )
         matrix WHEELS1 ( 1 0 0 0 1 0 0 0 1 -0.000100732 0.465734 1.849 )
         matrix WHEELS2 ( 1 0 0 0 1 0 0 0 1 -0.000100692 0.465734 0.943869 )
         matrix CONNEC_ROD ( 1 0 0 0 1 0 0 0 1 0.000100692 -0.169641 0.0112051 )
         matrix COUPL_ROD1 ( 1 0 0 0 0.958494 -0.285112 0 0.285112 0.958494 0.000100712 -0.178968 0.0114442 )
         matrix COUPL_ROD2 ( 1 0 0 0 0.963324 0.268339 0 -0.268339 0.963324 0 -0.0574915 0.978737 )
         matrix WHEELS3 ( 1 0 0 0 1 0 0 0 1 -0.000100619 0.465734 -0.738316 )
         matrix WHEELS4 ( 1 0 0 0 1 0 0 0 1 -0.00010058 0.465734 -1.63263 )
         matrix BILANCINO ( 1 0 0 0 0.968859 0.247615 0 -0.247615 0.968859 0 0.67549 2.0708 )
         matrix BIELLE_CIL ( 1 0 0 0 1 0 0 0 1 -0.000123162 0.762281 2.11056 )
         matrix BIELLE_CI0 ( 1 0 0 0 1 0 0 0 1 -0.000123162 0.826905 2.10482 )
         matrix SOST_BIEL_ ( 1 0 0 0 0.968859 0.247615 0 -0.247615 0.968859 0 0.885885 0.144911 )
         matrix STEPS ( 1 0 0 0 1 0 0 0 1 0.000223756 0.677047 -2.50429 )
         matrix SERBATOI ( 1 0 0 0 1 0 0 0 1 5.50747e-005 1.91455 1.51411 )
         matrix CAB_E ( 1 0 0 0 1 0 0 0 1 0 2.15691 -1.89651 )
         matrix CAB_INT ( 1 0 0 0 1 0 0 0 1 0 2.15605 -1.89764 )
         matrix CAMINO ( 1 0 0 0 1 0 0 0 1 0 3.2418 2.49266 )
         matrix CARBONIERA ( 1 0 0 0 1 0 0 0 1 0 1.70676 -3.11957 )
         matrix CASSE_ACQU ( 1 0 0 0 1 0 0 0 1 0 1.3135 1.0649 )
         matrix CASSE_ACQ0 ( 1 0 0 0 1 0 0 0 1 0 2.83779 0.374306 )
         matrix CILINDRO_D ( 1 0 0 0 1 0 0 0 1 0.680851 0.634267 2.62373 )
         matrix CILINDRO_S ( -1 0 0 0 1 0 0 0 -1 0 3.05485 0.370902 )
         matrix CILINDRO_0 ( 1 0 0 0 1 0 0 0 1 -0.680999 0.634267 2.62373 )
         matrix MANIGLIETT ( 1 0 0 0 1 0 0 0 1 0 1.94149 3.27145 )
         matrix MARCATURA ( 1 0 0 0 1 0 0 0 1 0 1.10162 2.5041 )
         matrix PARASOLE ( 1 0 0 0 1 0 0 0 1 7.96318e-005 2.86727 -0.812931 )
         matrix RESPING ( 1 0 0 0 1 0 0 0 1 0.000134125 0.615113 0.00396265 )
         matrix RUBINETTO ( 1 0 0 0 1 0 0 0 1 0.713191 2.05025 0 )
         matrix SOSTEGNI ( 1 0 0 0 1 0 0 0 1 0 0.96481 2.63779 )
         matrix TUBO1 ( 1 0 0 0 1 0 0 0 1 0.501836 2.21097 1.05257 )
         matrix RESPINGENT ( 1 0 0 0 1 0 0 0 1 -0.000243187 0.601995 0.000232473 )
         matrix FARO_A ( 1 0 0 0 1 0 0 0 1 0 2.86362 2.94188 )
         matrix FISCHIO ( 0.772309 0 -0.635246 0 1 0 0.635246 0 0.772309 0.00150821 3.60004 -1.46268 )
         matrix FARO_P ( 1 0 0 0 1 0 0 0 1 0 2.93698 -2.97352 )
      )
```   

Osservate i primi nove valori di ogni riga: non c'è lo stesso schema

```
    1 0 0 0 1 0 0 0 1
```   

in tutte le righe e la regular expression che avevo predisposto non era in grado di rilevarle.

Aggiusto la regular expression in questo modo:

```
   /s*matrixs+(S+)s+(s*([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)s+([-d.e]+)/i
```   

riprovo e la locomotiva passa senza problemi.
Per chi non lo sapesse, una "regular expression" non è una "passeggiata del gatto sulla tastiera", ma è un modo per indicare uno schema (il termine esatto sarebbe pattern) di stringa che voglio cercare all'interno di un testo. Comodissima in casi come questo dove devo cercare delle strutture ben specifiche.

Il vero problema però non era tanto quello di correggere il pattern (è bastato un minuto), ma è il significato di quei nove numeri che non mi era affatto misterioso.
In uno dei primi post ho detto che quei numeri non ci interessavano, ma adesso era arrivato il momento di usarli.

In ogni caso, visto che la locomotiva dava il risultato corretto (pura fortuna), faccio il furbo e rimando il programma a Guido.
Di li a poco mi restituisce un ponte, con una rampa in curva, dicendomi che con quello venivano fuori dei numeri strani: mi aveva sgamato!

Vado a rispolverare il libro di "Geometria" dell'università... quello che non aveva neanche una figura! 


## Le rotazioni

Eccomi qua con il "Cavalieri D'Oro" (è il cognome dell'autore), libro che a suo tempo mi aveva sorpreso per due motivi: innanzitutto era scritto in "bella calligrafia" e non con i comuni caratteri tipografici; poi c'era il fatto che pur essendo un testo che si intitolava "Geometria" non aveva figure.

Il professore, di fronte allo stupore di parecchi per la mancanza delle figure, ci fece una domanda: "come faccio a rappresentare uno spazio, che so, a 6 dimensioni in una figura?"

Ci spiegò che in pratica avremmo studiato dell'algebra che avrebbe permesso le trasformazioni che nella geometria piana (a 2 dimensioni) si fanno con riga e compasso.
Sono le regole matematiche che stanno dentro i motori di rendering (al tempo non erano cosa comune e non avremmo mai fatto menzione alla cosa).

Per chi volesse approfondire la cosa ho trovato questa interessante dispensa dell'università di Pisa http://medialab.di.unipi.it/web/IUM/Fondamenti/cap8.htm che fa parte di un più completo trattato sulla grafica a computer (http://medialab.di.unipi.it/web/IUM/Fondamenti/)

Non spaventatevi, non ho affatto intenzione di mettermi a spiegare certi argomenti, anche perchè non essendo fresco di studio (sono passati quasi 30 anni) potrei essere facilmente "messo in buca".

Torniamo a noi, prendiamo i valori "standard" della matrix e disponiamoli così:

```
   1 0 0
   0 1 0
   0 0 1
 ```  

E' una matrice unitaria; moltiplicando un array con tre coordinate per questa matrice si ottiene di nuovo l'array di partenza, ma negli altri casi?

Intanto chi volesse sapere cosa significa moltiplicare delle matrici (l'array è un caso particolare di matrice con 1 sola riga o colonna) può dare un'occhiata a questa voce di Wikipedia: http://it.wikipedia.org/wiki/Moltiplicazione_di_matrici.

In generale con quella moltiplicazione si effettua una trasformazione che nel caso specifico sono delle rotazioni attorno ai 3 assi; anche qui per i dettagli c'è sempre Wikipedia: http://it.wikipedia.org/wiki/Rotazione_%28matematica%29

Guido ha modellato alcuni componenti diritti e poi li ha ruotati per portarli al giusto orientamento: nel caso della locomotiva questi pezzi, pur ruotati, non uscivano dall'ingombro di altri pezzi e quindi il bounding box generale non cambiava; ma nel caso del ponte la lunga rampa in curva cambiava (e di parecchio) il bounding box.

Lavorando con le matrici si possono fare anche altre trasformazioni esemplificate in questo documento: http://projects.ivl.disco.unimib.it/eig/02a_trasformazioni.pdf

Moltiplicare delle matrici non è difficile, è solo pesante come elaborazione: per ogni punto e per ogni livello della gerarchia vanno eseguite 9 moltiplicazioni in virgola mobile che moltiplicate per alcune migliaia di punti... fanno un bel po'!
Già avevo qualche problema di eccessiva lentezza con la E428 di Vittorio elaborando la quale il browser si lamentava che "uno script sta impiegando troppo tempo..."; figuriamoci se gli facevo fare anche tutti quei calcoli.

Qualche trucchetto, come verificare subito se la matrice è unitaria o riorganizzando il parser precalcolando alcune cose, mi ha permesso di stare nei limiti anche con oggetti di 11000 poligoni.

Così oggi questo programma è disponibile per tutti e si spiega perchè Shape File Manger crei gli .sd con il bounding box vuoto.

Spero di non aver tediato nessuno (ma chi ve l'ha fatto fare di leggere fin qui? :smiley: ). 

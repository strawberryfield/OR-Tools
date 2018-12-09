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
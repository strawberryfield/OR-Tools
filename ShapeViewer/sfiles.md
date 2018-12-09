### Giocare con i file .s

# Scherzosa analisi dei file .s 

Originariamente pubblicato su [TrainSimHobby](http://www.trainsimhobby.net/forum/viewtopic.php?t=10514)

## Introduzione

Fosse Sandro intitolerebbe questo post "Indiana Jones e il Bounding Box"; in effetti la storia che vado a raccontare ha parecchio dell'avventuroso.
Per indole io sono uno che i problemi invece che affrontarli trova sempre il modo di scansarli, ma questa volta le cose sono andate un po' diversamente...

Iniziamo con il descrivere il problema: creare un file .sd per i miei modelli 3d creati con gmax (che a differenza di altri programmi "dedicati" non lo crea automaticamente).

Una soluzione abbastanza comoda era utilizzare "Shape File Manager" che crea un file dove bisogna solo impostare se ci sono texture alternative (neve, notte, stagioni...) cosa che il programma non può sapere, e il "bounding box" che invece il programma potrebbe ricavare dal modello.
L'unico sistema per evitare di calcolarsi l'ingombro dell'oggetto è quello di aprire il modello con "Shape Viewer", far apparire la finestra con i dati calcolati del bounding box e riportarli manualmente (e nell'ordine giusto, che è diverso da quello con cui li presenta shape viewer!) all'interno del file .sd
Ce n'è fin troppo per creare inutili e fastidiosi errori.

Dò una rapida occhiata al contenuto del file .s che contiene il modello della stazioncina che stavo realizzando e quasi all'inizio trovo questa interessante sequenza 
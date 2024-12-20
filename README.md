# GettingStarted
Förslag på hur man kan hantera stora meddelanden som går via MassTransit.
Utmaningen är att consumers och meddelanden ska lämnas oförändrade. 

Notera i MassTransit-konfigurationen att det finns en factory som ersätter defaultimplementationen av serialiserare och deserialiserare.

Det finns en Serialize och en DeSerializer om serialiserar respektive deserialiserar meddelanden. 
Om meddelandet är större än tröskelvärdet sparas det ner (InMemoryClaimCheckStorage) och ett claimcheck meddelande skickas istället vidare med en referens till det nedsparade meddelandet.

När MassTransit på mottagarsidan ser ett meddelande med claimcheck, hämtas det riktiga meddelandet upp via referensen och ersätter claimcheck-meddelandet.

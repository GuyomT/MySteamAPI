### MySteamAPI

## Launching the project

To launch the project, you need to have docker and docker-compose installed on your machine. Then, you can run the following command:

```bash
docker compose up --build
```

Go on your browser and type the following URL:

```bash
http://localhost:7001swagger/index.html
```

Unfortunately, the project is not currently working due to issue related to the database and SqlServer on Mac M3 chip.

In order to use the StripeAPI, you can use the PaymentProcessing service on swagger and use one of the following tokens that you can find on the following documentation: : [Stripe documentation](https://docs.stripe.com/testing?locale=fr-FR&testing-method=tokens)
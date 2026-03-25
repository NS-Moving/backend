# No Stress Moving Contact Form Backend

## Description
Sends an email notiificaiton through a smtp using a Gmail account.

## API: 
### POST contact/submit   
Sends emails to all addresses in environment variable

```
request body:
{
  name: string,
  phoneNumber: string,
  email: string,
  date: string,
  description: string,
  addOns: string,
  movingToAddress: string,
  movingFromAddress: string
}
```

### GET warmup
Endpoint meant to wake up the Funciton App to reduce cold start up time. Meant to be called on page load.

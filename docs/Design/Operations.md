<!---
Title: Operations
NavigationTitle: Operations
BreadcrumbTitle: Operations
ShowInNavigation: false
ShowInSidebar: true
NoSidebar: false
Excerpt: Describes operations, the basis for all signalling and processing in the architecture

--->
## Operations
Operations are the basis of all processing in the architecture. An operation contains all the information required to complete a step or entire business process.
## validations
Operations have validation functionality provided by the fluent validation library. Every operation can self validate to check if it has the minimum data required for a high probatiliy of successdul execution. Because an operation understands the minimum data required to succesfully execute and is defined by the implemntor, this is the logical location for the validation code. This pattern allows the creator to maximize the probability of success prior to incurring any of the costs of transport, prepation or transmission.


```csharp

/// Define my operation

public class notifySomeone : Operation
{
string email;
}

var tellthem = new NotifySomeone ("you@here.com");

// Validate before executing.
tellthem .Validate();

// send message
servicebus.send(tellthem)

// OR calling a transparent proxy
myApi.notify(tellthem);

 ```


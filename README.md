[![Build status](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/ContextRegExMatch?branch=master)](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/ContextRegExMatch/branch/master)

##Description
The Context RegEx Match component contains functionality to read a specific value in the context of a message, perform a regular expression match on that value and in case of a successful match promote a fixed value to another part of the message context.

The component is useful when receiving a message with a promoted value in its context that one then want to evaluate rules against to for example simplify filtering in routing of messages.

| Parameter               | Description                                                                                                                   | Type | Validation|
| ------------------------|-------------------------------------------------------------------------------------------------------------------------------|------|-----------|
|Pattern To Match|The RegEx pattern used to match the specific context value against.|String|Required|
|Context Property To Match|Namespace and value of the context property that the match should execute against. Should be in format 'http://foo.bar#value'.|String|Required|
|Context Property To Set|Namespace and value of the context property that should be used to the set the value. Should be in format 'http://foo.bar#value'.|String|Required|
|Value To Set|The value to set if the match is successful.|String|Required|

##Remarks
If the component receives null when trying to reading the value that the match should be performed against an exception will occur.

If the message context that the components is trying to promote is not found, if for example that schema is not deployed, an exception will occur.


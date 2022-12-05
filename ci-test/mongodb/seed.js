db = connect("localhost:27017/EvenTransit");

db.Event.insertMany([
    {
        "_id": UUID("cb7cf701-8c80-59d6-8a57-e2fbe3a3cec0"),
        "Name": "OneSuccessOneFailEvent",
        "Services": [
            {
                "Name": "FailTest",
                "Url": "http://mountebank:5025/api/failtest",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            },
            {
                "Name": "SuccessTest",
                "Url": "http://mountebank:5025/api/successtest",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            }
        ]
    },
    {
        "_id": UUID("31315035-bf92-4d93-8a6f-396c48c94d3e"),
        "Name": "OneSuccessEvent",
        "Services": [
            {
                "Name": "SuccessTest",
                "Url": "http://mountebank:5025/api/successtest",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            }
        ]
    },
    {
        "_id": UUID("948e10ee-ef6d-4262-8d9f-6fb78d9aadaf"),
        "Name": "OneFailEvent",
        "Services": [
            {
                "Name": "FailTest",
                "Url": "http://mountebank:5025/api/failtest",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            }
        ]
    },
    {
        "_id": UUID("e045ec7e-2e1e-49dc-853f-b4b5c2ed3833"),
        "Name": "LogTest",
        "Services": [
            {
                "Name": "Fail",
                "Url": "http://localhost/api/failtest",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            },
            {
                "Name": "Success",
                "Url": "http://host.docker.internal:5025/api/failtest",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            }
        ]
    },
    {
        "_id": UUID("d929ae33-3284-4638-8864-8811ac2df0fb"),
        "Name": "MessageTest",
        "Services": [
            {
                "Name": "FailTest",
                "Url": "http://localhost/api/v1",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            }
        ]
    },
    {
        "_id": UUID("7d903f45-daec-4f89-a0a5-af2752dd7d60"),
        "Name": "AddServiceTest",
        "Services": []
    },
    {
        "_id": UUID("acbeb3b0-79dd-45a6-9090-4b13b5aba751"),
        "Name": "UpdateServiceTest",
        "Services": [
            {
                "Name": "UpdateTest",
                "Url": "http://localhost/api/v1",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "5"
                }
            }
        ]
    },
    {
        "_id": UUID("5a66da96-f28b-4a62-85d3-be29038619e2"),
        "Name": "UniqueTest",
        "Services": []
    },
    {
        "_id": UUID("1f7e6f44-07d6-4874-a508-dd3093da2001"),
        "Name": "AdditionalProps",
        "Services": [
            {
                "Name": "AdditionalProps",
                "Url": "http://localhost/api/v1/{{PARAMS}}",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "{{HEADER}}"
                }
            }
        ]
    },
    {
        "_id": UUID("e24d158c-da34-4861-aaf9-68a4207e32d3"),
        "Name": "DeleteEvents",
        "Services": [
            {
                "Name": "DeleteEvents",
                "Url": "http://localhost/api/v1/{{PARAMS}}",
                "Timeout": 15,
                "Method": "POST",
                "Headers": {
                    "userId": "{{HEADER}}"
                }
            }
        ]
    },
    {
        "_id": UUID("b6af25f2-8499-4197-8961-5f2e3e54a900"),
        "Name": "FailFiveHundredCode",
        "Services": [{
            "Name": "FailFiveHundredCode",
            "Url": "http://mountebank:5025/api/failtest500",
            "Timeout": 15,
            "Method": "POST",
            "Headers": {}
        }]
    },
    {
        "_id": UUID("b6af25f2-8499-4197-8961-5f2e3e54a901"),
        "Name": "FailFourHundredEightCode",
        "Services": [{
            "Name": "FailFourHundredEightCode",
            "Url": "http://mountebank:5025/api/failtest408",
            "Timeout": 15,
            "Method": "POST",
            "Headers": {}
        }]
    },
    {
        "_id": UUID("b6af25f2-8499-4197-8961-5f2e3e54a902"),
        "Name": "FailFourHundredTwentyNineCode",
        "Services": [{
            "Name": "FailFourHundredTwentyNineCode",
            "Url": "http://mountebank:5025/api/failtest429",
            "Timeout": 15,
            "Method": "POST",
            "Headers": {}
        }]
    }
])

db.EventLogStatistic.insertMany([
    {
        "_id": UUID("ec1b595c-836f-4569-a0d9-9d5024f722d2"),
        "EventId": UUID("cb7cf701-8c80-59d6-8a57-e2fbe3a3cec0"),
        "EventName": "OneSuccessOneFailEvent",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("b6af25f2-8499-4197-8961-4f2e3e54a978"),
        "EventId": UUID("31315035-bf92-4d93-8a6f-396c48c94d3e"),
        "EventName": "OneSuccessEvent",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("4e529412-ca4d-41a9-9881-ad59327af1a2"),
        "EventId": UUID("948e10ee-ef6d-4262-8d9f-6fb78d9aadaf"),
        "EventName": "OneFailEvent",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("2731e647-baa9-4d5d-a8cc-72fc2400a6e7"),
        "EventId": UUID("e045ec7e-2e1e-49dc-853f-b4b5c2ed3833"),
        "EventName": "LogTest",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("43d50c19-5ef0-4e6c-8aa5-d790f4bf44de"),
        "EventId": UUID("d929ae33-3284-4638-8864-8811ac2df0fb"),
        "EventName": "MessageTest",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("e62d503e-93bb-44ee-a967-faa7e4b1b79f"),
        "EventId": UUID("7d903f45-daec-4f89-a0a5-af2752dd7d60"),
        "EventName": "AddServiceTest",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("63882cc5-c9ff-4183-9ed2-91c78bbcdde3"),
        "EventId": UUID("acbeb3b0-79dd-45a6-9090-4b13b5aba751"),
        "EventName": "UpdateServiceTest",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("9c9ef291-1584-41de-9af0-96d13268a464"),
        "EventId": UUID("5a66da96-f28b-4a62-85d3-be29038619e2"),
        "EventName": "UniqueTest",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("e71557f7-eb2b-4914-bac0-41a23be1ad52"),
        "EventId": UUID("1f7e6f44-07d6-4874-a508-dd3093da2001"),
        "EventName": "AdditionalProps",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("c2358492-3d7c-48fe-bf6f-6bc52b5eb118"),
        "EventId": UUID("e24d158c-da34-4861-aaf9-68a4207e32d3"),
        "EventName": "DeleteEvents",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("ec1b595c-836f-4569-a0d9-2d6024f722d1"),
        "EventId": UUID("b6af25f2-8499-4197-8961-5f2e3e54a900"),
        "EventName": "FailFiveHundredCode",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("ec1b595c-836f-4569-a0d9-2d6024f722d2"),
        "EventId": UUID("b6af25f2-8499-4197-8961-5f2e3e54a901"),
        "EventName": "FailFourHundredEightCode",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    },
    {
        "_id": UUID("ec1b595c-836f-4569-a0d9-2d6024f722d3"),
        "EventId": UUID("b6af25f2-8499-4197-8961-5f2e3e54a902"),
        "EventName": "FailFourHundredTwentyNineCode",
        "FailCount": 0,
        "ServiceCount": 0,
        "SuccessCount": 0
    }
])

db.LogStatistic.insertMany([
    {
        "_id": UUID("12f0abc2-55a5-4db9-927b-276e006ef5f6"),
        "Date": new Date(),
        "FailCount": 0,
        "SuccessCount": 0
    }
])

db.Logs.insertMany([
    {
        "_id": UUID("534e6522-5543-4014-a520-650639f195b6"),
        "CreatedOn": ISODate("2021-10-03T20:50:35.528Z"),
        "Details": {
            "Message": null,
            "Request": {
                "Url": "http://localhost/api/failtest",
                "Timeout": 15,
                "Body": "null",
                "Headers": {
                    "userId": "5"
                }
            },
            "Response": {
                "IsSuccess": true,
                "StatusCode": 400,
                "Response": "{\n    \"message\": \"Fail Test\"\n}"
            }
        },
        "EventName": "LogTest",
        "LogType": 2,
        "ServiceName": "Fail"
    },
    {
        "_id": UUID("ee66f2ce-7b0b-49c7-aa0f-d6692bdae863"),
        "CreatedOn": ISODate("2021-10-07T20:50:35.528Z"),
        "Details": {
            "Message": null,
            "Request": {
                "Url": "http://localhost/api/failtest",
                "Timeout": 15,
                "Body": "null",
                "Headers": {
                    "userId": "5"
                }
            },
            "Response": {
                "IsSuccess": true,
                "StatusCode": 400,
                "Response": "{\n    \"message\": \"Fail Test\"\n}"
            }
        },
        "EventName": "LogTest",
        "LogType": 2,
        "ServiceName": "Fail"
    },
    {
        "_id": UUID("27b30e69-f0f9-405a-b176-5569d5e5377b"),
        "CreatedOn": ISODate("2021-10-07T20:50:35.528Z"),
        "Details": {
            "Message": null,
            "Request": {
                "Url": "http://localhost/api/failtest",
                "Timeout": 15,
                "Body": "null",
                "Headers": {
                    "userId": "5"
                }
            },
            "Response": {
                "IsSuccess": true,
                "StatusCode": 400,
                "Response": "{\n    \"message\": \"Fail Test\"\n}"
            }
        },
        "EventName": "LogTest",
        "LogType": 2,
        "ServiceName": "Fail"
    },
    {
        "_id": UUID("62199e82-aeaf-4b42-957e-b7ae8a42be50"),
        "CreatedOn": ISODate("2021-10-25T20:50:35.528Z"),
        "Details": {
            "Message": null,
            "Request": {
                "Url": "http://localhost/api/failtest",
                "Timeout": 15,
                "Body": "null",
                "Headers": {
                    "userId": "5"
                }
            },
            "Response": {
                "IsSuccess": true,
                "StatusCode": 400,
                "Response": "{\n    \"message\": \"Fail Test\"\n}"
            }
        },
        "EventName": "LogTest",
        "LogType": 2,
        "ServiceName": "Fail"
    },
    {
        "_id": UUID("6877b9c1-0113-4270-99e8-9622fd53d9de"),
        "CreatedOn": ISODate("2021-10-06T20:50:35.528Z"),
        "Details": {
            "Message": null,
            "Request": {
                "Url": "http://localhost/api/successtest",
                "Timeout": 15,
                "Body": "null",
                "Headers": {
                    "userId": "5"
                }
            },
            "Response": {
                "IsSuccess": true,
                "StatusCode": 200,
                "Response": "{\n    \"message\": \"Success Test\"\n}"
            }
        },
        "EventName": "LogTest",
        "LogType": 1,
        "ServiceName": "Success"
    }
])
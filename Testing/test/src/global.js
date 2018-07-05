Test.assert(() => parseInt("34") == 34, "ParseInt string");
Test.assert(() => isNaN(parseInt("abc")), "ParseInt invalid string");
Test.assert(() => parseInt("3.4") == 3, "ParseInt float string");
Test.assert(() => parseInt(3.56) == 3, "ParseInt float");

Test.assert(() => parseFloat("3.5") == 3.5, "ParseFloat string");
Test.assert(() => isNaN(parseFloat("abc")), "ParseFloat invalid string");

Test.assert(() => encodeURI("https://mozilla.org/?x=шеллы") == "https://mozilla.org/?x=%D1%88%D0%B5%D0%BB%D0%BB%D1%8B", "EncodeURI");
Test.assert(() => decodeURI("https://mozilla.org/?x=%D1%88%D0%B5%D0%BB%D0%BB%D1%8B") == "https://mozilla.org/?x=шеллы", "DecodeURI");

Test.assert(() => encodeURIComponent("?x=шеллы") == "%3Fx%3D%D1%88%D0%B5%D0%BB%D0%BB%D1%8B", "EncodeURIComponent");
Test.assert(() => decodeURIComponent("%3Fx%3D%D1%88%D0%B5%D0%BB%D0%BB%D1%8B") == "?x=шеллы", "DecodeURIComponent");

Test.assert(() => eval("3 + 5") == 8, "Eval simple addition");
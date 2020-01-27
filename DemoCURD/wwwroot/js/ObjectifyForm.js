function objectifyForm(serializedArray) {

    let object = {};

    for (var i = 0; i < serializedArray.length; i++) {

        object[serializedArray[i]['name']] = serializedArray[i]['value'];

    }

    return object;
}

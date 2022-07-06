function getInvokeOutsideAlerterFunction(outsideAlerterDotNetInstance, id) {
    return async function invokeStaticOutsideAlerter(evt) {
        const alerterElement = document.getElementById(id);
        if (alerterElement !== null) {
            const containedInAlerter = alerterElement.contains(evt.target);
            if (!containedInAlerter) {
                await outsideAlerterDotNetInstance.invokeMethodAsync('InvokeClickOutside');
            }
        }
    }
}

function setupOutsideAlerter(outsideAlerterDotNetInstance, id) {
    document.removeEventListener('mousedown', getInvokeOutsideAlerterFunction(outsideAlerterDotNetInstance, id));
    document.addEventListener('mousedown', getInvokeOutsideAlerterFunction(outsideAlerterDotNetInstance, id));
}

function cleanupOutsideAlerter(outsideAlerterDotNetInstance, id) {
    document.removeEventListener('mousedown', getInvokeOutsideAlerterFunction(outsideAlerterDotNetInstance, id));
}

let uixGid = function(e) {
    return parseInt(e.getAttribute("data-uix-gid"))
}

// called by onclick
// noinspection JSUnusedGlobalSymbols
function UIX_ButtonClick(e) {
    let id = uixGid(e)
    
    engine.trigger("UixButtonClick", id)
}

function UIX_DoBrrClass(parent, clazz, func) {
    let inputs = parent.querySelectorAll(clazz)
    for (let i = 0; i < inputs.length; i++) {
        func(inputs[i])
    }
}

let uix_VisHandlers = {}
let uix_TextHandlers = {}
let uix_TextAlignHandlers = {}
let uix_ToggleHandlers = {}

function UIX_AddGidEventsToChildren(_e) {
    UIX_DoBrrClass(_e, ".uix_button_mark", function(e) {
        let gid = uixGid(e)
        uix_VisHandlers[gid] = e
        uix_TextHandlers[gid] = function(t) { e.innerText = t }
    })
    
    UIX_DoBrrClass(_e, ".uix_label_mark", function(e) {
        let gid = uixGid(e)
        uix_VisHandlers[gid] = e
        uix_TextHandlers[gid] = function(t) { e.innerText = t }
    })
    
    // todo: toggles
}

engine.on("UixControlSetVisible", function(id, v) {
    let handler = uix_VisHandlers[id]
    if (handler) handler.style.visibility = v ? "visible" : "collapse"
})

engine.on("UixControlSetText", function(id, t) {
    let handler = uix_TextHandlers[id]
    if (handler) handler(t)
})

engine.on("UixControlSetTestAnchor", function(id, a) {
    let handler = uix_TextAlignHandlers[id]
    if (handler) handler(a)
})

engine.on("UixControlSetSelected", function(id, v) {
    let handler = uix_ToggleHandlers[id]
    if (handler) handler(v)
})

let handlerMap = {}
let visibilityHandlerMap = {}

function inp_toggle_uix(_obj) {
    let result = {}
    inp_toggle.call(result, _obj)
    
    let cat = _obj.getAttribute('data-uix-cat')
    let entry = _obj.getAttribute('data-uix-entry')
    
    handlerMap[[cat, entry]] = function (v) {
        result.updateValue(v)
    }
    
    visibilityHandlerMap[[cat, entry]] = _obj

    let oldMouseDown = result.mouseDown
    _obj.removeEventListener('mousedown', oldMouseDown)

    let newMouseDown = function() {
        console.log(`Called new mouseDown for ${cat}/${entry} - value is ${result.value}`)

        oldMouseDown()
        
        let isNewChecked = "true" === result.value.toLowerCase()

        engine.trigger("UixSetBoolSetting", cat, entry, isNewChecked)
    }
    
    _obj.addEventListener('mousedown', newMouseDown)
    
    return result
}

function inp_dropdown_uix(_obj) {
    let result = {}
    inp_dropdown.call(result, _obj)
    
    let cat = _obj.getAttribute('data-uix-cat')
    let entry = _obj.getAttribute('data-uix-entry')

    handlerMap[[cat, entry]] = function (v) {
        result.updateValue(v)
    }
    visibilityHandlerMap[[cat, entry]] = _obj

    let oldSelectValue = result.SelectValue
    result.SelectValue = function(_e){
        oldSelectValue(_e)
        
        let value = Number.parseFloat(result.value)
        engine.trigger('UixSetDropdownSetting', cat, entry, value);
    }
    
    result.updateOptions()
    
    return result
}

function inp_slider_uix(_obj) {
    let result = {}
    inp_slider.call(result, _obj)

    let cat = _obj.getAttribute('data-uix-cat')
    let entry = _obj.getAttribute('data-uix-entry')

    handlerMap[[cat, entry]] = function (v) {
        result.updateValue(v)
    }
    visibilityHandlerMap[[cat, entry]] = _obj
    
    let oldMouseDown = result.mouseDown
    result.mouseDown = function(_e) {
        oldMouseDown(_e)

        document.addEventListener('mousemove', result.mouseMove)
        document.addEventListener('mouseup', result.mouseUp)
    }
    
    _obj.removeEventListener('mousedown', oldMouseDown)
    _obj.addEventListener('mousedown', result.mouseDown)

    let oldMouseMove = result.mouseMove
    result.mouseMove = function(_e, _write){
        oldMouseMove(_e)

        let value = result.value
        engine.trigger('UixSetSlider', cat, entry, value, _write);
    }
    
    let oldMouseUp = result.mouseUp
    result.mouseUp = function(_e) {
        oldMouseUp(_e)
        
        document.removeEventListener('mousemove', result.mouseMove)
        document.removeEventListener('mouseup', result.mouseUp)
    }

    // don't want these hanging around forever and spamming events from every sneeze
    document.removeEventListener('mousemove', oldMouseMove)
    document.removeEventListener('mouseup', oldMouseUp)
    
    return result
}

function inp_number_uix(_obj) {
    let result = {}
    inp_number.call(result, _obj)

    let cat = _obj.getAttribute('data-uix-cat')
    let entry = _obj.getAttribute('data-uix-entry')

    handlerMap[[cat, entry]] = function (v) {
        result.updateValue(v)
    }
    visibilityHandlerMap[[cat, entry]] = _obj
    
    result.updateValue = function(_v){
        result.value = _v
        
        if (result.mode === "int"){
            _obj.innerHTML = result.caption + ": " + result.value;
        } else {
            _obj.innerHTML = result.caption + ": " + result.value.toFixed(4);
        }

        let value = result.value
        engine.trigger('UixSetNumber', cat, entry, value);
    }


    return result
}

function inp_string_uix(_obj) {
    let cat = _obj.getAttribute('data-uix-cat')
    let entry = _obj.getAttribute('data-uix-entry')

    handlerMap[[cat, entry]] = function (v) {
        _obj.value = v
    }
    visibilityHandlerMap[[cat, entry]] = _obj
}

let currentCategoryBody = null

function UIX_updateCategoriesBodies() {
    handlerMap = {}
    visibilityHandlerMap = {}
    
    let wrapper = document.getElementsByClassName("wrapper")[0]
    if (currentCategoryBody !== null) {
        currentCategoryBody.parentNode.removeChild(currentCategoryBody)
    }

    engine.call('UixGetSettingsHtml').then(function(res) {
        wrapper.insertAdjacentHTML('afterbegin', res)
        let firstChild = wrapper.firstChild;
        currentCategoryBody = firstChild
        UIX_DoBrrClass(firstChild, ".inp_toggle", inp_toggle_uix)
        UIX_DoBrrClass(firstChild, ".inp_dropdown", inp_dropdown_uix)
        UIX_DoBrrClass(firstChild, ".inp_slider", inp_slider_uix)
        UIX_DoBrrClass(firstChild, ".inp_number", inp_number_uix)
        UIX_DoBrrClass(firstChild, ".inp_search", inp_string_uix)
        
        UIX_AddGidEventsToChildren(firstChild)
        
        engine.trigger("UixResubmitSettingVisibilities")
    }, function(err) {
        console.log("Engine call to get settings html body failed with error " + err)
    })
}

engine.on("UixSettingValueUpdated", function(cat, entry, value) {
    console.log(`Got update notice for ${cat}/${entry} = ${value}`)
    let handler = handlerMap[[cat, entry]]
    if (handler) {
        handler(value)
    } else {
        console.log(`UIX setting ${cat}/${entry} has no update handler, bug?`)
    }
})

engine.on("UixSettingVisibilityUpdated", function(cat, entry, visible) {
    let handler = visibilityHandlerMap[[cat, entry]]
    if (handler) {
        handler.style.visibility = visible ? "visible" : "collapse"
    }
})

function UIX_StringSettingUpdated(_e) {
    let cat = _e.getAttribute('data-uix-cat')
    let entry = _e.getAttribute('data-uix-entry')

    engine.trigger("UixSetString", cat, entry, _e.value)
}

function switchUixCat(_id, _e) {
    let buttons = document.querySelectorAll('#uix-settings .filter-option')
    let categories = document.querySelectorAll('#uix-settings .settings-categorie')
    
    for(let i = 0; i < buttons.length; i++){
        buttons[i].classList.remove('active')
    }

    for(let i = 0; i < categories.length; i++){
        categories[i].classList.remove('active')
    }

    _e.classList.add('active')
    document.getElementById(_id).classList.add('active')
}

UIX_updateCategoriesBodies()

let settingsButton = document.getElementsByClassName("tab_btn_settings")[0]
settingsButton.insertAdjacentHTML('afterend', `<div class="toolbar-btn tab_btn_settings button" onclick="changeTab(\'uix-settings\', this);"><img src="uix-resource:twemoji-melon.svg"/>Mòwóds</div>`)

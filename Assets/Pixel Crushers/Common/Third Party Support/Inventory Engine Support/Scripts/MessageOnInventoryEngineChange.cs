// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

namespace PixelCrushers.InventoryEngineSupport
{

    /// <summary>
    /// Add to an inventory. Sends a message to the Pixel Crushers Message System 
    /// when the inventory changes.
    /// </summary>
    public class MessageOnInventoryEngineContentChange : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {

        [Tooltip("Message to send.")]
        public StringField message = new StringField("InventoryEngineContentChanged");

        [Tooltip("Parameter to send. If blank, sends the name of this inventory.")]
        public StringField parameter = new StringField();

        public string runtimeParameter { get { return !StringField.IsNullOrEmpty(parameter) ? parameter.value : name; } }

        protected virtual void OnEnable()
        { 
            this.MMEventStartListening<MMInventoryEvent>();
        }

        protected virtual void OnDisable()
        { 
            this.MMEventStopListening<MMInventoryEvent>();
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            switch (eventType.InventoryEventType)
            {
                case MMInventoryEventType.ContentChanged:
                    MessageSystem.SendMessage(this, message, runtimeParameter);
                    break;
            }
        }
    }
}

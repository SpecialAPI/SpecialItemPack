using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack.Shapeshifting
{
    class ShrineShapeshifter : BreachShrineController
    {
        public override void Interact(PlayerController interactor)
        {
            base.Interact(interactor);
            if (TextBoxManager.HasTextBox(this.talkPoint))
            {
                return;
            }
            base.StartCoroutine(this.HandleShrineConversation(interactor));
        }

        private IEnumerator HandleShrineConversation(PlayerController interactor)
        {
            Shapeshift shapeshift = GetNextShapeshift(interactor);
            string targetDisplayKey = "#SHRINE_SHAPESHIFTER_TABLET";
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(targetDisplayKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            yield return null;
            string declineKey = (interactor.GetComponent<ShapeShiftBehaviour>() != null) ? "#SHRINE_SHAPESHIFTER_CLEAR" : "#SHRINE_SHAPESHIFTER_DECLINE";
            GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(shapeshift.acceptKey), StringTableManager.GetString(declineKey));
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            if (selectedResponse == 0)
            {
                if (interactor.GetComponent<ShapeShiftBehaviour>() != null)
                {
                    interactor.GetComponent<ShapeShiftBehaviour>().RemoveShapeshift();
                }
                ShapeShiftBehaviour.AddToTarget(shapeshift.behaviour, interactor);
                GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#SHRINE_SHAPESHIFTER_SHAPESHIFTED"), StringTableManager.GetString("#SHRINE_SHAPESHIFTER_CUR_SHAPESHIFT") + StringTableManager.GetString(shapeshift.name),
                    SpriteBuilder.itemCollection, spriteId, UINotificationController.NotificationColor.PURPLE, false, false);
                AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", base.gameObject);
            }
            else
            {
                if(interactor.GetComponent<ShapeShiftBehaviour>() != null)
                {
                    interactor.GetComponent<ShapeShiftBehaviour>().RemoveShapeshift();
                    GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#SHRINE_SHAPESHIFTER_SHAPESHIFTED"), StringTableManager.GetString("#SHRINE_SHAPESHIFTER_CUR_SHAPESHIFT") + 
                        StringTableManager.GetString("#SHRINE_SHAPESHIFTER_NORMAL_SHAPESHIFT"), SpriteBuilder.itemCollection, spriteId, UINotificationController.NotificationColor.PURPLE, false, false);
                    AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", base.gameObject);
                }
            }
            yield break;
        }

        public static Shapeshift GetNextShapeshift(PlayerController interactor)
        {
            Shapeshift result = shapeshifts[0];
            if(interactor.GetComponent<ShapeShiftBehaviour>() != null)
            {
                Type type = interactor.GetComponent<ShapeShiftBehaviour>().GetType();
                int i = 0;
                foreach (Shapeshift shapeshift in shapeshifts)
                {
                    i++;
                    if (type == shapeshift.behaviour)
                    {
                        break;
                    }
                }
                if(i >= shapeshifts.Count)
                {
                    i = 0;
                }
                result = shapeshifts[i];
            }
            return result;
        }

        public Transform talkPoint;
        public static List<Shapeshift> shapeshifts = new List<Shapeshift>();
        public static int spriteId;
    }

    struct Shapeshift
    {
        public string acceptKey;
        public string name;
        public Type behaviour;
    }
}

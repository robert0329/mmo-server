﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Swaelo_Server
{
    public class CharacterCamera
    {
        public WorldRenderer Game { get; private set; }
        public Camera Camera { get; private set; }

        public float StandingCameraOffset { get; set; }
        public float CrouchingCameraOffset { get; set; }
        public float ProneCameraOffset { get; set; }
        public CharacterController Character { get; private set; }
        public bool UseCameraSmoothing { get; set; }

        public CharacterCamera(CharacterController character, Camera camera, WorldRenderer game)
        {
            Camera = camera;
            Game = game;
            Character = character;
            UseCameraSmoothing = true;
            StandingCameraOffset = 0.7f;
            CrouchingCameraOffset = 0.4f;
            ProneCameraOffset = 0.1f;
        }

        public void Update(float dt)
        {
            //Only turn if the mouse is controlled by the game.
            if (!Game.IsMouseVisible)
            {
                Camera.Yaw((200 - Game.MouseInput.X) * dt * .12f);
                Camera.Pitch((200 - Game.MouseInput.Y) * dt * .12f);
            }

            ////Rotate the camera of the character based on the support velocity, if a support with velocity exists.
            ////This can be very disorienting in some cases; that's why it is off by default!
            //if (Character.SupportFinder.HasSupport)
            //{
            //    SupportData? data;
            //    if (Character.SupportFinder.HasTraction)
            //        data = Character.SupportFinder.TractionData;
            //    else
            //        data = Character.SupportFinder.SupportData;
            //    var support = data.Value.SupportObject as EntityCollidable;
            //    if (support != null && !support.Entity.IsDynamic) //Having the view turned by dynamic entities is extremely confusing for the most part.
            //    {
            //        float dot = Vector3.Dot(support.Entity.AngularVelocity, Character.Body.OrientationMatrix.Up);
            //        Camera.Yaw(dot * dt);
            //    }
            //}

            float cameraOffset;
            switch (Character.StanceManager.CurrentStance)
            {
                case Stance.Prone:
                    cameraOffset = ProneCameraOffset;
                    break;
                case Stance.Crouching:
                    cameraOffset = CrouchingCameraOffset;
                    break;
                default:
                    cameraOffset = StandingCameraOffset;
                    break;
            }
            if (UseCameraSmoothing)
            {
                //First, find where the camera is expected to be based on the last position and the current velocity.
                //Note: if the character were a free-floating 6DOF character, this would need to include an angular velocity contribution.
                //And of course, the camera orientation would be based on the character's orientation.

                //We use the space's time step since, in the demos, the simulation moves forward one time step per frame.
                //The passed-in dt, in contrast, does not necessarily correspond to a simulated state and tends to make the camera jittery.
                var spaceDt = Character.Space != null ? Character.Space.TimeStepSettings.TimeStepDuration : dt;
                Camera.Position = Camera.Position + Character.Body.LinearVelocity * spaceDt;
                //Now compute where it should be according the physical body of the character.
                Vector3 up = Character.Body.OrientationMatrix.Up;
                Vector3 bodyPosition = Character.Body.BufferedStates.InterpolatedStates.Position;

                Vector3 goalPosition = bodyPosition + up * cameraOffset;

                //Usually, the camera position and the goal will be very close, if not matching completely.
                //However, if the character steps or has its position otherwise modified, then they will not match.
                //In this case, we need to correct the camera position.

                //To do this, first note that we can't correct infinite errors.  We need to define a bounding region that is relative to the character
                //in which the camera can interpolate around.  The most common discontinuous motions are those of upstepping and downstepping.
                //In downstepping, the character can teleport up to the character's MaximumStepHeight downwards.
                //In upstepping, the character can teleport up to the character's MaximumStepHeight upwards, and the body's CollisionMargin horizontally.
                //Picking those as bounds creates a constraining cylinder.

                Vector3 error = goalPosition - Camera.Position;
                float verticalError = Vector3.Dot(error, up);
                Vector3 horizontalError = error - verticalError * up;
                //Clamp the vertical component of the camera position within the bounding cylinder.
                if (verticalError > Character.StepManager.MaximumStepHeight)
                {
                    Camera.Position -= up * (Character.StepManager.MaximumStepHeight - verticalError);
                    verticalError = Character.StepManager.MaximumStepHeight;
                }
                else if (verticalError < -Character.StepManager.MaximumStepHeight)
                {
                    Camera.Position -= up * (-Character.StepManager.MaximumStepHeight - verticalError);
                    verticalError = -Character.StepManager.MaximumStepHeight;
                }
                //Clamp the horizontal distance too.
                float horizontalErrorLength = horizontalError.LengthSquared();
                float margin = Character.Body.CollisionInformation.Shape.CollisionMargin;
                if (horizontalErrorLength > margin * margin)
                {
                    Vector3 previousHorizontalError = horizontalError;
                    Vector3.Multiply(ref horizontalError, margin / (float)Math.Sqrt(horizontalErrorLength), out horizontalError);
                    Camera.Position -= horizontalError - previousHorizontalError;
                }
                //Now that the error/camera position is known to lie within the constraining cylinder, we can perform a smooth correction.

                //This removes a portion of the error each frame.
                //Note that this is not framerate independent.  If fixed time step is not enabled,
                //a different smoothing method should be applied to the final error values.
                //float errorCorrectionFactor = .3f;

                //This version is framerate independent, although it is more expensive.
                float errorCorrectionFactor = (float)(1 - Math.Pow(.000000001, dt));
                Camera.Position += up * (verticalError * errorCorrectionFactor);
                Camera.Position += horizontalError * errorCorrectionFactor;


            }
            else
            {
                Camera.Position = Character.Body.Position + cameraOffset * Character.Body.OrientationMatrix.Up;
            }
        }
    }
}
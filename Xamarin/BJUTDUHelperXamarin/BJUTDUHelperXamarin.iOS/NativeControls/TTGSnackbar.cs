using System;
using Foundation;
using UIKit;

namespace TTGSnackBar
{
    public enum TTGSnackbarAnimationType
    {
        FadeInFadeOut,
        SlideFromBottomToTop,
        SlideFromBottomBackToBottom,
        SlideFromLeftToRight,
        SlideFromRightToLeft,
    }

    public enum TTGSnackbarLocation
    {
        Bottom,
        Top,
    }

    public class TTGSnackbar : UIView
    {

        /// Snackbar action button max width.
        private const float snackbarActionButtonMaxWidth = 64;

        // Snackbar action button min width.
        private const float snackbarActionButtonMinWidth = 22;

        // Action callback.
        public Action<TTGSnackbar> ActionBlock { get; set; } = null;

        // Dismiss block
        public Action<TTGSnackbar> DismissBlock { get; set; }

        // Snackbar display duration. Default is Short - 1 second.
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);

        // Snackbar animation type. Default is SlideFromBottomBackToBottom.
        public TTGSnackbarAnimationType AnimationType = TTGSnackbarAnimationType.SlideFromLeftToRight;

        // Snackbar location
        public TTGSnackbarLocation LocationType = TTGSnackbarLocation.Bottom;

        // Show and hide animation duration. Default is 0.3
        public float AnimationDuration = 0.3f;

        private float _cornerRadius = 0;
        public float CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = value;
                if (_cornerRadius > Height)
                {
                    _cornerRadius = Height / 2;
                }
                if (_cornerRadius < 0)
                    _cornerRadius = 0;

                this.Layer.CornerRadius = _cornerRadius;
                this.Layer.MasksToBounds = true;
            }
        }

        /// Top margin. Default is 4
        private float _topMargin = 20;
        public float TopMargin
        {
            get { return _topMargin; }
            set
            {
                _topMargin = value; if (topMarginConstraint != null) { topMarginConstraint.Constant = _topMargin; this.LayoutIfNeeded(); }
            }
        }

        /// Left margin. Default is 4
        private float _leftMargin = 0;
        public float LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; if (leftMarginConstraint != null) { leftMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }
        }

        private float _rightMargin = 0;
        public float RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; if (rightMarginConstraint != null) { rightMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }
        }

        /// Bottom margin. Default is 4
        private float _bottomMargin = 0;
        public float BottomMargin
        {
            get { return _bottomMargin; }
            set { _bottomMargin = value; if (bottomMarginConstraint != null) { bottomMarginConstraint.Constant = _bottomMargin; this.LayoutIfNeeded(); } }
        }

        private float _height = 44;
        public float Height
        {
            get { return _height; }
            set { _height = value; if (heightConstraint != null) { heightConstraint.Constant = _height; this.LayoutIfNeeded(); } }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; if (this.MessageLabel != null) { this.MessageLabel.Text = _message; } }
        }

        private UIColor _messageTextColor = UIColor.White;
        public UIColor MessageTextColor
        {
            get { return _messageTextColor; }
            set { _messageTextColor = value; this.MessageLabel.TextColor = _messageTextColor; }
        }

        private UIFont _messageTextFont = UIFont.BoldSystemFontOfSize(14);
        public UIFont MessageTextFont
        {
            get { return _messageTextFont; }
            set { _messageTextFont = value; this.MessageLabel.Font = _messageTextFont; }
        }

        private UITextAlignment _messageTextAlign;
        public UITextAlignment MessageTextAlign
        {
            get { return _messageTextAlign; }
            set { _messageTextAlign = value; this.MessageLabel.TextAlignment = _messageTextAlign; }
        }

        private string _actionText;
        public string ActionText
        {
            get { return _actionText; }
            set { _actionText = value; if (this.ActionButton != null) { this.ActionButton.SetTitle(_actionText, UIControlState.Normal); } }
        }


        // Action button title color. Default is white.
        private UIColor _actionTextColor = UIColor.White;
        public UIColor ActionTextColor
        {
            get { return _actionTextColor; }
            set { _actionTextColor = value; this.ActionButton.SetTitleColor(_actionTextColor, UIControlState.Normal); }
        }


        // First action text font. Default is Bold system font (14).
        private UIFont _actionTextFont = UIFont.BoldSystemFontOfSize(14);
        public UIFont ActionTextFont
        {
            get { return _actionTextFont; }
            set { _actionTextFont = value; this.ActionButton.TitleLabel.Font = _actionTextFont; }
        }


        public UILabel MessageLabel;
        public UIView SeperateView;
        public UIButton ActionButton;
        public UIActivityIndicatorView ActivityIndicatorView;

        // Timer to dismiss the snackbar.
        private NSTimer dismissTimer;

        // Constraints.
        private NSLayoutConstraint heightConstraint;
        private NSLayoutConstraint leftMarginConstraint;
        private NSLayoutConstraint rightMarginConstraint;
        private NSLayoutConstraint bottomMarginConstraint;
        private NSLayoutConstraint topMarginConstraint;
        private NSLayoutConstraint actionButtonWidthConstraint;

        /// <summary>
        /// Show a single message like an Android snackbar.
        /// - parameter message:  Message text.
        /// </summary>
        public TTGSnackbar(string message) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Message = message;
            configure();
        }

        /// <summary>
        /// Show a message with action button.
        /// - parameter message:     Message text.
        /// - parameter actionText:  Action button title.
        /// - parameter actionBlock: Action callback closure.
        /// - returns: Void
        /// </summary>
        public TTGSnackbar(string message, string actionText, Action<TTGSnackbar> ttgAction) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Message = message;
            this.ActionText = actionText;
            this.ActionBlock = ttgAction;

            configure();
        }

        /// <summary>
        /// Show a custom message with action button.
        /// - parameter message:          Message text.
        /// - parameter actionText:       Action button title.
        /// - parameter messageFont:      Message label font.
        /// - parameter actionButtonFont: Action button font.
        /// - parameter actionBlock:      Action callback closure.
        /// - returns: Void
        /// </summary>
        public TTGSnackbar(string message, string actionText, UIFont messageFont, UIFont actionTextFont, Action<TTGSnackbar> ttgAction) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Message = message;
            this.ActionText = actionText;
            this.ActionBlock = ttgAction;
            this.MessageTextFont = messageFont;
            this.ActionTextFont = actionTextFont;

            configure();
        }


        /// <summary>
        /// Show the snackbar.
        /// </summary>
        public void Show()
        {
            // Only show once
            if (this.Superview != null)
                return;

            // Create dismiss timer
            dismissTimer = NSTimer.CreateScheduledTimer(Duration, (t) => Dismiss());

            // Show or hide action button

            SeperateView.Hidden = ActionButton.Hidden;

            actionButtonWidthConstraint.Constant = ActionButton.Hidden ? 0 : TTGSnackbar.snackbarActionButtonMinWidth;

            this.LayoutIfNeeded();

            var localSuperView = UIApplication.SharedApplication.KeyWindow;
            if (localSuperView != null)
            {
                localSuperView.AddSubview(this);

                topMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Top,
                    1,
                    TopMargin);

                heightConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.GreaterThanOrEqual,
                    null,
                    NSLayoutAttribute.NoAttribute,
                    1,
                    Height);

                leftMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Left,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Left,
                    1,
                    LeftMargin);

                rightMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Right,
                    1,
                    -RightMargin);

                bottomMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Bottom,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Bottom,
                    1,
                    -BottomMargin);

                // Avoid the "UIView-Encapsulated-Layout-Height" constraint conflicts
                // http://stackoverflow.com/questions/25059443/what-is-nslayoutconstraint-uiview-encapsulated-layout-height-and-how-should-i
                leftMarginConstraint.Priority = 999;
                rightMarginConstraint.Priority = 999;

                this.AddConstraint(heightConstraint);
                localSuperView.AddConstraint(leftMarginConstraint);
                localSuperView.AddConstraint(rightMarginConstraint);

                switch (LocationType)
                {
                    case TTGSnackbarLocation.Top:
                        localSuperView.AddConstraint(topMarginConstraint);
                        break;
                    default:
                        localSuperView.AddConstraint(bottomMarginConstraint);
                        break;
                }


                // Show 
                showWithAnimation();
            }
            else
            {
                Console.WriteLine("TTGSnackbar needs a keyWindows to display.");
            }
        }

        /// <summary>
        /// Dismiss the snackbar manually..
        /// </summary>
        public void Dismiss()
        {
            this.dismissAnimated(true);
        }

        /// <summary>
        /// Configure this instance.
        /// </summary>
        private void configure()
        {
            this.TranslatesAutoresizingMaskIntoConstraints = false;
            this.BackgroundColor = UIColor.DarkGray;
            this.Layer.CornerRadius = CornerRadius;
            this.Layer.MasksToBounds = true;

            MessageLabel = new UILabel();
            MessageLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            MessageLabel.TextColor = UIColor.White;
            MessageLabel.Font = MessageTextFont;
            MessageLabel.BackgroundColor = UIColor.Clear;
            MessageLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
            MessageLabel.Lines = 2;
            MessageLabel.TextAlignment = UITextAlignment.Left;
            MessageLabel.Text = Message;

            this.AddSubview(MessageLabel);

            ActionButton = new UIButton();
            ActionButton.TranslatesAutoresizingMaskIntoConstraints = false;
            ActionButton.BackgroundColor = UIColor.Clear;
            ActionButton.TitleLabel.Font = ActionTextFont;
            ActionButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
            ActionButton.SetTitle(ActionText, UIControlState.Normal);
            ActionButton.SetTitleColor(ActionTextColor, UIControlState.Normal);
            ActionButton.TouchUpInside += (s, e) =>
            {
                if (!ActionButton.Hidden && ActionButton.Title(UIControlState.Normal) != String.Empty && ActionButton != null)
                {
                    ActionBlock(this);
                    dismissAnimated(true);
                }
            };

            this.AddSubview(ActionButton);


            SeperateView = new UIView();
            SeperateView.TranslatesAutoresizingMaskIntoConstraints = false;
            SeperateView.BackgroundColor = UIColor.Gray;

            this.AddSubview(SeperateView);

            ActivityIndicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
            ActivityIndicatorView.TranslatesAutoresizingMaskIntoConstraints = false;
            ActivityIndicatorView.StopAnimating();

            this.AddSubview(ActivityIndicatorView);

            // Add constraints

            var hConstraints = NSLayoutConstraint.FromVisualFormat(
                "H:|-12-[messageLabel]-2-[seperateView(0.5)]-2-[actionButton(>=40@999)]-10-|",
                0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] {
                        MessageLabel,
                        SeperateView,
                        ActionButton
                }, new NSObject[] {
                    new NSString("messageLabel"),
                    new NSString("seperateView"),
                    new NSString("actionButton")
                })
            );


            var vConstraintsForMessageLabel = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[messageLabel]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { MessageLabel }, new NSObject[] { new NSString("messageLabel") })
            );

            var vConstraintsForSeperateView = NSLayoutConstraint.FromVisualFormat(
                "V:|-4-[seperateView]-4-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { SeperateView }, new NSObject[] { new NSString("seperateView") })
            );

            var vConstraintsForActionButton = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[actionButton]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { ActionButton }, new NSObject[] { new NSString("actionButton") })
            );




            actionButtonWidthConstraint = NSLayoutConstraint.Create(ActionButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, TTGSnackbar.snackbarActionButtonMinWidth);


            var vConstraintsForActivityIndicatorView = NSLayoutConstraint.FromVisualFormat(
                "V:|-2-[activityIndicatorView]-2-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { ActivityIndicatorView }, new NSObject[] { new NSString("activityIndicatorView") })
            );

            var hConstraintsForActivityIndicatorView = NSLayoutConstraint.FromVisualFormat(
                "H:[activityIndicatorView]-2-|",
                0,
                new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { ActivityIndicatorView },
                    new NSObject[] { new NSString("activityIndicatorView") })
            );


            ActionButton.AddConstraint(actionButtonWidthConstraint);

            this.AddConstraints(hConstraints);
            this.AddConstraints(vConstraintsForMessageLabel);
            this.AddConstraints(vConstraintsForSeperateView);
            this.AddConstraints(vConstraintsForActionButton);
            this.AddConstraints(vConstraintsForActivityIndicatorView);
            this.AddConstraints(hConstraintsForActivityIndicatorView);
        }

        /// <summary>
        /// Invalid the dismiss timer.
        /// </summary>
        private void invalidDismissTimer()
        {
            if (dismissTimer != null)
            {
                dismissTimer.Invalidate();
                dismissTimer = null;
            }
        }

        /// <summary>
        /// If dismiss with animation.
        /// </summary>
        private void dismissAnimated(bool animated)
        {
            invalidDismissTimer();

            ActivityIndicatorView.StopAnimating();

            nfloat superViewWidth = 0;

            if (Superview != null)
                superViewWidth = Superview.Frame.Width;

            if (!animated)
            {
                DismissAndPerformAction();
                return;
            }

            Action animationBlock = () => { };

            switch (AnimationType)
            {
                case TTGSnackbarAnimationType.FadeInFadeOut:
                    animationBlock = () => { this.Alpha = 0; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomBackToBottom:
                    animationBlock = () => { bottomMarginConstraint.Constant = Height; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomToTop:
                    animationBlock = () => { this.Alpha = 0; bottomMarginConstraint.Constant = -Height - BottomMargin; };
                    break;
                case TTGSnackbarAnimationType.SlideFromLeftToRight:
                    animationBlock = () => { leftMarginConstraint.Constant = LeftMargin + superViewWidth; rightMarginConstraint.Constant = -RightMargin + superViewWidth; };
                    break;
                case TTGSnackbarAnimationType.SlideFromRightToLeft:
                    animationBlock = () =>
                    {
                        leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                        rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    };
                    break;
            };

            this.SetNeedsLayout();

            UIView.Animate(AnimationDuration, 0, UIViewAnimationOptions.CurveEaseIn, animationBlock, DismissAndPerformAction);
        }

        void DismissAndPerformAction()
        {
            if (DismissBlock != null)
            {
                DismissBlock(this);
            }

            this.RemoveFromSuperview();
        }

        /// <summary>
        /// Shows with animation.
        /// </summary>
        private void showWithAnimation()
        {
            Action animationBlock = () => { this.LayoutIfNeeded(); };
            var superViewWidth = Superview.Frame.Width;

            switch (AnimationType)
            {
                case TTGSnackbarAnimationType.FadeInFadeOut:
                    this.Alpha = 0;
                    this.SetNeedsLayout();

                    animationBlock = () => { this.Alpha = 1; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomBackToBottom:
                case TTGSnackbarAnimationType.SlideFromBottomToTop:
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
                case TTGSnackbarAnimationType.SlideFromLeftToRight:
                    leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
                case TTGSnackbarAnimationType.SlideFromRightToLeft:
                    leftMarginConstraint.Constant = LeftMargin + superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin + superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
            };

            // Final state
            bottomMarginConstraint.Constant = -BottomMargin;
            leftMarginConstraint.Constant = LeftMargin;
            rightMarginConstraint.Constant = -RightMargin;
            topMarginConstraint.Constant = TopMargin;

            UIView.AnimateNotify(
                    AnimationDuration,
                    0,
                    0.7f,
                    5f,
                    UIViewAnimationOptions.CurveEaseInOut,
                      animationBlock,
                    null
                );
        }

        private void doAction(UIButton button)
        {
            // Call action block first
            if (button == ActionButton)
            {
                ActionBlock(this);
            }

            dismissAnimated(true);
        }
    }
}



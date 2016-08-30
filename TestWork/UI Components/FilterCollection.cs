using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TestWork
{
	public class FilterCollectionView : UICollectionView
	{
		public delegate void FilterSelectedDelegate(Filter filter);

		public event FilterSelectedDelegate onSelectFilter;

		List<Filter> filters;

		public FilterCollectionView(IntPtr pointer):base(pointer)
		{
		}

		public FilterCollectionView(CGRect frame,List<Filter> filters,UIImage image):base(frame,new LineLayout())
		{
			AllowsSelection = true;
			AllowsMultipleSelection = false;
			this.UserInteractionEnabled = true;
			if (image == null)
			{
				image= new UIImage("Picture.jpg");
			}
			else {
				image = ImageTransformation.MaxResizeImage(image, 100, 100); 
			}

			DataSource = new FilterCollectionDataSource(filters,image);
			this.filters = filters;
			Delegate = new FilterCollectionDelegate();
			BackgroundColor = UIColor.Clear;
		}



		public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var filter = filters[indexPath.Row];
			var cell = collectionView.CellForItem(indexPath) as FilterCell;
			if (onSelectFilter != null)
				onSelectFilter(filter);
		}

		public void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath) as FilterCell;
		}
	}

	public class FilterCollectionDelegate : UICollectionViewDelegate
	{
		// PROBLEM: none of the methods in this class ever gets called
		public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (FilterCell)collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = UIColor.Green;
		}

		public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (FilterCell)collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = UIColor.Gray;
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			(collectionView as FilterCollectionView).ItemSelected(collectionView, indexPath);
			var cell = (FilterCell)collectionView.CellForItem(indexPath);
			cell.ContentView.Layer.BorderWidth = 4;
		}

		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			(collectionView as FilterCollectionView).DeselectItem(indexPath,true);
			var cell = (FilterCell)collectionView.CellForItem(indexPath);
			if(cell!=null)
				cell.ContentView.Layer.BorderWidth = 1;
		}
	}

	public class FilterCollectionDataSource:UICollectionViewDataSource
	{
		static NSString filterCellId = new NSString("FilterCell");
		static NSString headerId = new NSString("Header");
		UIImage image;
		List<Filter> filters;

		public FilterCollectionDataSource(List<Filter> filters,UIImage image)
		{
			this.filters = filters;
			this.image = image;

		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return filters.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var filterCell = (FilterCell)collectionView.DequeueReusableCell(filterCellId, indexPath);

			var filter = filters[indexPath.Row];

			filterCell.imageView.Image = filter.ProcessingImage(image);

			filterCell.Label.Text = filter.Name;

			return filterCell;
		}

	}

	public partial class FilterCell : UICollectionViewCell
	{
		public UILabel Label { get; private set; }
		public UIImageView imageView;

		[Export("initWithFrame:")]
		public FilterCell(CGRect frame) : base(frame)
		{
			Label = new UILabel(new CGRect(CGPoint.Empty, frame.Size))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.BoldSystemFontOfSize(12f),
				ShadowColor = UIColor.Black.ColorWithAlpha(0.5f),
				ShadowOffset = new CGSize(1, 1),
				TextColor = UIColor.White
			};

			imageView = new UIImageView(new CGRect(CGPoint.Empty, frame.Size))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				ContentMode = UIViewContentMode.ScaleAspectFill
			};

			ContentView.Layer.CornerRadius = 15;
			ContentView.ClipsToBounds = true;
			ContentView.AddSubview(imageView);
			ContentView.AddSubview(Label);
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
		}
	}

	public class GridLayout : UICollectionViewFlowLayout
	{
		public GridLayout()
		{
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath path)
		{
			return base.LayoutAttributesForItem(path);
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			return base.LayoutAttributesForElementsInRect(rect);
		}
	}

	public partial class LineLayout : UICollectionViewFlowLayout
	{
		public const float ITEM_SIZE = 70.0f;
		public const int ACTIVE_DISTANCE = 60;
		public const float ZOOM_FACTOR = 0.0f;

		public LineLayout()
		{
			ItemSize = new CGSize(ITEM_SIZE, ITEM_SIZE);
			ScrollDirection = UICollectionViewScrollDirection.Horizontal;
			SectionInset = new UIEdgeInsets(40, 0.0f, 40, 0.0f);
			MinimumLineSpacing = 20.0f;
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			var array = base.LayoutAttributesForElementsInRect(rect);
			var visibleRect = new CGRect(CollectionView.ContentOffset, CollectionView.Bounds.Size);

			/*foreach (var attributes in array)
			{
				if (attributes.Frame.IntersectsWith(rect))
				{
					float distance = (float)visibleRect.GetMidX() - (float)attributes.Center.X;
					float normalizedDistance = distance / ACTIVE_DISTANCE;
					if (Math.Abs(distance) < ACTIVE_DISTANCE)
					{
						float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs(normalizedDistance));
						attributes.Transform3D = CATransform3D.MakeScale(zoom, zoom, 1.0f);
						attributes.ZIndex = 1;
					}
				}
			}*/
			return array;
		}

		public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
		{
			float offSetAdjustment = float.MaxValue;
			float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
			CGRect targetRect = new CGRect(proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
			var array = base.LayoutAttributesForElementsInRect(targetRect);
			foreach (var layoutAttributes in array)
			{
				float itemHorizontalCenter = (float)layoutAttributes.Center.X;
				if (Math.Abs(itemHorizontalCenter - horizontalCenter) < Math.Abs(offSetAdjustment))
				{
					offSetAdjustment = itemHorizontalCenter - horizontalCenter;
				}
			}
			return new CGPoint(proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
		}
	}



}


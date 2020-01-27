import { Guid } from "../api/new/dto/Guid";
import { FeedbackProps } from "../commonComponents/Feedback/Feedback";
import { FeedbackType } from "../commonComponents/Feedback/FeedbackType";
import { FeaturesData } from "../typings/FeaturesData";

class FeatureAppearanceImpl {
  private data: FeaturesData;
  private userId: Guid;
  private shopId: Guid;

  public set(data: FeaturesData, userId: Guid, shopId: Guid): void {
    this.data = data;
    this.userId = userId;
    this.shopId = shopId;
  }

  public shouldShow(type: FeedbackType): boolean {
    return this.data && this.data.activeFeatures && this.data.activeFeatures.includes(type);
  }

  public activate(type: FeedbackType) {
    this.data.featuresCallbacks.activate(type);
  }

  public getProps(type: FeedbackType): FeedbackProps {
    return {
      shopId: this.shopId,
      userId: this.userId,
      type,
      onClick: this.data.featuresCallbacks.setCompleted,
      onClose: this.data.featuresCallbacks.markAsSeen,
    };
  }

  public getUserInfo() {
    return {
      shopId: this.shopId,
      userId: this.userId,
    };
  }
}

export const FeatureAppearance = new FeatureAppearanceImpl();

import { FeedbackType } from "../commonComponents/Feedback/FeedbackType";

export interface FeaturesData {
  activeFeatures: FeedbackType[];
  featuresCallbacks: FeaturesCallbacks;
}

export interface FeaturesCallbacks {
  activate: (name: string) => void;
  markAsSeen: (name: string) => void;
  setCompleted: (name: string) => void;
}

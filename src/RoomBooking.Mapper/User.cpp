template <>
struct mapper<RoomBooking::User>
{
	static M::Entity on_create()
	{
		return shared::create_user(Agent, shared::LOCAL_SYSTEM_NAME);
	}

	static void on_update_Username(entity root, const option<string> &value)
	{
		if (value)
			root.ensure_subject_of(HaveRelation, Name.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, Name);
	}

	static void on_update_Email(entity root, const option<string> &value)
	{
		if (value)
			root.ensure_subject_of(HaveRelation, EmailAddress.ensure_string(*value));
        else
            root.ensure_not_subject_of(HaveRelation, EmailAddress);
	}

	static void on_delete(entity root)
	{
		root.untrack_including_relations();
	}

	static void invalidate(change_type type, const entity &obj, const entity::changes &changes, entity::invset &inval)
	{
		const auto F = obj.features() + (changes.features_assigned() ? changes.features() : featureset{});

		shared::inval_user_relation(F, obj, inval, changes, Agent, true);

		if (Name.test(F) || EmailAddress.test(F))
		{
			from(obj).object_of(HaveRelation).having_subject(UserRelation).for_each(inval);
		}
	}

	static bool refresh(const entity &root, RoomBooking::User::refresh_batch &changes)
	{
		if (!shared::refresh_user_relation(root, Agent, shared::LOCAL_SYSTEM_NAME, true))
		{
			return false;
		}

		const auto username = from(root).subject_of(HaveRelation).having_object(Name).first();
		const auto email = from(root).subject_of(HaveRelation).having_object(EmailAddress).first();

		changes.Username(username ? username->string() : none);
		changes.Email(email ? email->string() : none);

		return true;
	}
};
